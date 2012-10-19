using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TechniqueGeneration
{
    class Program
    {
        public static string parameterCheck(int i)
        {
            return ((i != 0) ? "true" : "false");
        }

        public static void WriteTop(StreamWriter file, string techniqueName, int psVersion, string name, string VS)
        {
            file.WriteLine("technique " + techniqueName + psVersion + name);
            file.WriteLine("{");
            file.WriteLine("    pass P0");
            file.WriteLine("    {");
            if(techniqueName != "SkinnedMesh")
                file.WriteLine("        VertexShader = compile vs_3_0 " + VS + "();");
            else
                file.WriteLine("        VertexShader = " + VS + ";");
        }

        public static void WriteBottom(StreamWriter file, string pixelShader, string parameters)
        {
            file.WriteLine(pixelShader + parameters);
            file.WriteLine("    }");
            file.WriteLine("}");

            //Becomes more compact without this line, but makes the files a lot shorter for whatever that's worth :>
            //file.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.Write("TechniqueName: ");
            string techniqueName = Console.ReadLine();
            string VS = "";
            string PS = "";
            Console.Write("Pixel shader version: ");
            int psVersion = Int32.Parse(Console.ReadLine());

            
            if(psVersion != 2 && psVersion != 3)
                throw new NotSupportedException("Only pixel shader version 2 and 3 are available");

            StreamWriter file = new StreamWriter("C:/Keldyn/Trunk/Omicron/Shaders/Gen/" + techniqueName + psVersion + "Techniques.fx");

            if (techniqueName == "ShadowedScene")
            {
                VS = "SSVS";
                PS = "PSS";
            }
            else if (techniqueName == "ShadowedSceneInstanced")
            {
                VS = "VSInstanced";
                PS = "PSS";
            }
            else if (techniqueName == "Standard")
            {
                VS = "VS";
                PS = "PS";
            }
            else if (techniqueName == "SkinnedMesh")
            {
                VS = "(shaderArray[CurrentBoneCount])";
                PS = "PSS";
            }

            if (techniqueName != "Standard")
            {
                if (psVersion == 3)
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            for (int k = 0; k < 2; k++)
                                for (int l = 0; l < 2; l++)
                                    for (int m = 0; m < 2; m++)
                                        for (int n = 0; n < 2; n++)
                                        {
                                            string name = "";

                                            if (i == 0)
                                                name += "NoAmbient";
                                            else
                                                name += "Ambient";

                                            if (j == 0)
                                                name += "NoDiffuse";
                                            else
                                                name += "Diffuse";

                                            if (k == 0)
                                                name += "NoShadows";
                                            else
                                                name += "Shadows";

                                            if (l == 0)
                                                name += "NoWater";
                                            else
                                                name += "Water";

                                            if (m == 0)
                                                name += "NoFog";
                                            else
                                                name += "Fog";

                                            if (n == 0)
                                                name += "NoSpecular";
                                            else
                                                name += "Specular";

                                            WriteTop(file, techniqueName, psVersion, name, VS);

                                            string pixelshader = "        PixelShader = compile ps_" + psVersion + "_0 " + PS;
                                            string parameters = "(" + parameterCheck(i) + ", " + parameterCheck(j) + ", " + parameterCheck(k) + ", " + parameterCheck(l) + ", " + parameterCheck(m) + ", " + parameterCheck(n) + ");";

                                            WriteBottom(file, pixelshader, parameters);
                                        }
                }
                else
                {
                    for(int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            for(int k = 0; k < 2; k++)
                            {
                                string name = "";

                                if (i == 0)
                                    name += "NoAmbient";
                                else
                                    name += "Ambient";

                                if (j == 0)
                                    name += "NoDiffuse";
                                else
                                    name += "Diffuse";

                                name += "NoShadowsWater";

                                if (k == 0)
                                    name += "NoFog";
                                else
                                    name += "Fog";

                                name += "NoSpecular";

                                WriteTop(file, techniqueName, psVersion, name, VS);

                                string pixelshader = "        PixelShader = compile ps_" + psVersion + "_0 " + PS;
                                string parameters = "(" + parameterCheck(i) + ", " + parameterCheck(j) + ", false, true, true, false);";

                                WriteBottom(file, pixelshader, parameters);
                            }
                }
            }
            else
            {
                if (psVersion == 3)
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            for (int k = 0; k < 2; k++)
                                for (int l = 0; l < 2; l++)
                                    for (int m = 0; m < 2; m++)
                                    {
                                        string name = "AmbientDiffuse";

                                        if (i == 0)
                                            name += "NoShadows";
                                        else
                                            name += "Shadows";

                                        name += "WaterFog";

                                        if (j == 0)
                                            name += "NoSpecular";
                                        else
                                            name += "Specular";

                                        if (k == 0)
                                            name += "NoSplat1";
                                        else
                                            name += "Splat1";

                                        if (l == 0)
                                            name += "NoSplat2";
                                        else
                                            name += "Splat2";

                                        if (m == 0)
                                            name += "NoLowest";
                                        else
                                            name += "Lowest";

                                        WriteTop(file, techniqueName, psVersion, name, VS);

                                        string pixelshader = "        PixelShader = compile ps_" + psVersion + "_0 " + PS;
                                        string parameters = "(true, true, " + parameterCheck(i) + ", true, true, " + parameterCheck(j) + ", " + parameterCheck(k) + ", " + parameterCheck(l) + ", " + parameterCheck(m) + ");";

                                        WriteBottom(file, pixelshader, parameters);
                                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        string name = "AmbientDiffuseNoShadowsWaterFogNoSpecular";

                        if (i == 0)
                            name += "NoSplat1";
                        else
                            name += "Splat1";

                        name += "NoSplat2Lowest";

                        WriteTop(file, techniqueName, psVersion, name, VS);

                        string pixelshader = "        PixelShader = compile ps_" + psVersion + "_0 " + PS;
                        string parameters = "(true, true, false, true, true, false, " + parameterCheck(i) + ", false, true);";

                        WriteBottom(file, pixelshader, parameters);
                    }
                }
            }
            file.Close();
            Console.WriteLine("Press any key to continue... ");
            Console.ReadKey();
        }
    }
}
