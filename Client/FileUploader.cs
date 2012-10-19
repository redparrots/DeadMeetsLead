#define DEBUG_FILEUPLOADER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using FileInfoExtension;

namespace Client
{
    public static class FileUploader
    {
        public static void UploadZipFile(Uri uri, FileInfo fileToUpload)
        {
            ExecutePostRequest(uri, fileToUpload, "application/zip", "file1");
        }

        private static string ExecutePostRequest(Uri url, FileInfo fileToUpload, string fileMimeType, string fileFormKey)
        {
            if (fileToUpload == null)
#if DEBUG_FILEUPLOADER
                throw new Exception("Upload file is null.");
#else
                return;
#endif

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url.AbsoluteUri);
            request.Method = "POST";
            request.KeepAlive = true;
            string boundary = CreateFormDataBoundary();
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            Stream requestStream = null;
#if !DEBUG_FILEUPLOADER
            try
            {
#endif
            requestStream = request.GetRequestStream();
            fileToUpload.WriteMultipartFormData(requestStream, boundary, fileMimeType, fileFormKey);
            byte[] endBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
            requestStream.Write(endBytes, 0, endBytes.Length);
#if !DEBUG_FILEUPLOADER
            }
            catch (Exception)
            {
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
            }
#else
            requestStream.Close();
#endif
            using (WebResponse response = request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            };
        }

        /// <summary>
        /// Creates a multipart/form-data boundary.
        /// </summary>
        /// <returns>
        /// A dynamically generated form boundary for use in posting multipart/form-data requests.
        /// </returns>
        private static string CreateFormDataBoundary()
        {
            return "---------------------------" + DateTime.Now.Ticks.ToString("x");
        }
    }
}

namespace FileInfoExtension
{
    /// <summary>
    /// Extension methods for <see cref="System.IO.FileInfo"/>.
    /// </summary>
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Template for a file item in multipart/form-data format.
        /// </summary>
        public const string HeaderTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";

        /// <summary>
        /// Writes a file to a stream in multipart/form-data format.
        /// </summary>
        /// <param name="file">The file that should be written.</param>
        /// <param name="stream">The stream to which the file should be written.</param>
        /// <param name="mimeBoundary">The MIME multipart form boundary string.</param>
        /// <param name="mimeType">The MIME type of the file.</param>
        /// <param name="formKey">The name of the form parameter corresponding to the file upload.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if any parameter is <see langword="null" />.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown if <paramref name="mimeBoundary" />, <paramref name="mimeType" />,
        /// or <paramref name="formKey" /> is empty.
        /// </exception>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if <paramref name="file" /> does not exist.
        /// </exception>
        public static void WriteMultipartFormData(this FileInfo file, Stream stream, string mimeBoundary, string mimeType, string formKey)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            if (!file.Exists)
            {
                throw new FileNotFoundException("Unable to find file to write to stream.", file.FullName);
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            if (mimeBoundary == null)
            {
                throw new ArgumentNullException("mimeBoundary");
            }
            if (mimeBoundary.Length == 0)
            {
                throw new ArgumentException("MIME boundary may not be empty.", "mimeBoundary");
            }
            if (mimeType == null)
            {
                throw new ArgumentNullException("mimeType");
            }
            if (mimeType.Length == 0)
            {
                throw new ArgumentException("MIME type may not be empty.", "mimeType");
            }
            if (formKey == null)
            {
                throw new ArgumentNullException("formKey");
            }
            if (formKey.Length == 0)
            {
                throw new ArgumentException("Form key may not be empty.", "formKey");
            }
            string header = String.Format(HeaderTemplate, mimeBoundary, formKey, file.Name, mimeType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            stream.Write(headerbytes, 0, headerbytes.Length);
            using (FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();
            }
            byte[] newlineBytes = Encoding.UTF8.GetBytes("\r\n");
            stream.Write(newlineBytes, 0, newlineBytes.Length);
        }
    }
}
