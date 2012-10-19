
// .NET Installer script from http://tonaday.blogspot.com/2011/01/inno-setup-preparetoinstall-scripts.html

#define use_dotnetfx40
// German languagepack?
//#define use_dotnetfx35lp

// dotnet_Passive enabled shows the .NET/VC2010 installation progress, as it can take quite some time
#define dotnet_Passive

// Enable the required define(s) below if a local event function (prepended with Local) is used
//#define haveLocalPrepareToInstall
//#define haveLocalNeedRestart
//#define haveLocalNextButtonClick


#define SrcApp "..\InstallerWorkingDirectory\DeadMeetsLead.exe"
#define FileVerStr GetFileVersion(SrcApp)
#define StripBuild(str VerStr) Copy(VerStr, 1, RPos(".", VerStr)-1)
#define AppVerStr StripBuild(FileVerStr)

#define SetupScriptVersion '0'

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)

AppName={#AppName}
AppVersion={#AppVerStr}
AppVerName={#AppName} {#AppVerStr}
UninstallDisplayName={#AppName}
VersionInfoVersion={#FileVerStr}
VersionInfoTextVersion={#AppVerStr}
OutputBaseFilename={#AppName}-{#FileVerStr}

AppPublisher=Keldyn
AppPublisherURL=http://www.keldyn.com/
AppSupportURL=http://www.keldyn.com/
AppUpdatesURL=http://www.keldyn.com/
DefaultDirName={pf}\{#AppName}
DefaultGroupName={#AppName}
Compression=lzma
SolidCompression=yes
WizardImageFile=Installer1.bmp
WizardSmallImageFile=Installer2.bmp

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; Source: "..\InstallerWorkingDirectory\DeadMeetsLead.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\InstallerWorkingDirectory\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\DeadMeetsLead.exe"; WorkingDir: "{app}"
Name: "{group}\Leaditor"; Filename: "{app}\Leaditor.exe"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\DeadMeetsLead.exe"; WorkingDir: "{app}"; Tasks: desktopicon

[Dirs]
Name: "{app}"; Permissions: users-modify


[UninstallDelete]
Type: filesandordirs; Name: "{app}"


[Run]
Filename: "{app}\DeadMeetsLead.exe"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall

[Registry]
Root: HKCU; Subkey: "Software\Keldyn"; Flags: uninsdeletekeyifempty
Root: HKCU; Subkey: "Software\Keldyn\Dead Meets Lead"; Flags: uninsdeletekey


#include "scripts\products.iss"

#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"

#ifdef use_iis
#include "scripts\products\iis.iss"
#endif

#ifdef use_kb835732
#include "scripts\products\kb835732.iss"
#endif
#ifdef use_kb886903
#include "scripts\products\kb886903.iss"
#endif
#ifdef use_kb928366
#include "scripts\products\kb928366.iss"
#endif

#ifdef use_msi20
#include "scripts\products\msi20.iss"
#endif
#ifdef use_msi31
#include "scripts\products\msi31.iss"
#endif
#ifdef use_msi45
#include "scripts\products\msi45.iss"
#endif
#ifdef use_ie6
#include "scripts\products\ie6.iss"
#endif

#ifdef use_dotnetfx11
#include "scripts\products\dotnetfx11.iss"
#include "scripts\products\dotnetfx11lp.iss"
#include "scripts\products\dotnetfx11sp1.iss"
#endif

#ifdef use_dotnetfx20
#include "scripts\products\dotnetfx20.iss"
#ifdef use_dotnetfx20lp
#include "scripts\products\dotnetfx20lp.iss"
#endif
#include "scripts\products\dotnetfx20sp1.iss"
#ifdef use_dotnetfx20lp
#include "scripts\products\dotnetfx20sp1lp.iss"
#endif
#include "scripts\products\dotnetfx20sp2.iss"
#ifdef use_dotnetfx20lp
#include "scripts\products\dotnetfx20sp2lp.iss"
#endif
#endif

#ifdef use_dotnetfx35
#include "scripts\products\dotnetfx35.iss"
#ifdef use_dotnetfx35lp
#include "scripts\products\dotnetfx35lp.iss"
#endif
#include "scripts\products\dotnetfx35sp1.iss"
#ifdef use_dotnetfx35lp
#include "scripts\products\dotnetfx35sp1lp.iss"
#endif
#endif

#ifdef use_dotnetfx40
#include "scripts\products\dotnetfx40client.iss"
#include "scripts\products\dotnetfx40full.iss"
#endif
#ifdef use_vc2010
#include "scripts\products\vc2010.iss"
#endif

#ifdef use_mdac28
#include "scripts\products\mdac28.iss"
#endif
#ifdef use_jet4sp8
#include "scripts\products\jet4sp8.iss"
#endif
// SQL 3.5 Compact Edition
#ifdef use_scceruntime
#include "scripts\products\scceruntime.iss"
#endif
// SQL Express
#ifdef use_sql2005express
#include "scripts\products\sql2005express.iss"
#endif
#ifdef use_sql2008express
#include "scripts\products\sql2008express.iss"
#endif

[CustomMessages]
win2000sp3_title=Windows 2000 Service Pack 3
winxpsp2_title=Windows XP Service Pack 2
winxpsp3_title=Windows XP Service Pack 3

#expr SaveToFile(AddBackslash(SourcePath) + "Preprocessed"+AppName+SetupScriptVersion+".iss")

[Code]

function InitializeSetup(): Boolean;
begin
  Log('Init setup');
	//init windows version
	initwinversion();
	Log('Win version inited');
	//check if dotnetfx20 can be installed on this OS
	//if not minwinspversion(5, 0, 3) then begin
	//	MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('win2000sp3_title')]), mbError, MB_OK);
	//	exit;
	//end;
	if not minwinspversion(5, 1, 3) then begin
		MsgBox(FmtMessage(CustomMessage('depinstall_missing'), [CustomMessage('winxpsp3_title')]), mbError, MB_OK);
		exit;
	end;
	
#ifdef use_iis
	if (not iis()) then exit;
#endif	
	
#ifdef use_msi20
	msi20('2.0');
#endif
#ifdef use_msi31
	msi31('3.1');
#endif
#ifdef use_msi45
	msi45('4.5');
#endif
#ifdef use_ie6
	ie6('5.0.2919');
#endif
	
#ifdef use_dotnetfx11
	dotnetfx11();
#ifdef use_dotnetfx11lp
	dotnetfx11lp();
#endif
	dotnetfx11sp1();
#endif
#ifdef use_kb886903
	kb886903(); //better use windows update
#endif
#ifdef use_kb928366
	kb928366(); //better use windows update
#endif
	
	//install .netfx 2.0 sp2 if possible; if not sp1 if possible; if not .netfx 2.0
#ifdef use_dotnetfx20
	if minwinversion(5, 1) then begin
		dotnetfx20sp2();
#ifdef use_dotnetfx20lp
		dotnetfx20sp2lp();
#endif
	end else begin
		if minwinversion(5, 0) and minwinspversion(5, 0, 4) then begin
#ifdef use_kb835732
			kb835732();
#endif
			dotnetfx20sp1();
#ifdef use_dotnetfx20lp
			dotnetfx20sp1lp();
#endif
		end else begin
			dotnetfx20();
#ifdef use_dotnetfx20lp
			dotnetfx20lp();
#endif
		end;
	end;
#endif
	
	Log('Install .net 3.5');
#ifdef use_dotnetfx35
	//dotnetfx35();
#ifdef use_dotnetfx35lp
	dotnetfx35lp();
#endif
	dotnetfx35sp1();
#ifdef use_dotnetfx35lp
	dotnetfx35sp1lp();
#endif
#endif
	
	// If no .NET 4.0 framework found, install the smallest
#ifdef use_dotnetfx40
	if not dotnetfx40client(true) then
	    if not dotnetfx40full(true) then
	        dotnetfx40client(false);
	// Alternatively:
	// dotnetfx40full();
#endif

	// Visual C++ 2010 Redistributable
#ifdef use_vc2010
	vc2010();
#endif
	
#ifdef use_mdac28
	mdac28('2.7');
#endif
#ifdef use_jet4sp8
	jet4sp8('4.0.8015');
#endif
	// SQL 3.5 CE
#ifdef use_ssceruntime
	ssceruntime();
#endif
	// SQL Express
#ifdef use_sql2005express
	sql2005express();
#endif
#ifdef use_sql2008express
	sql2008express();
#endif

	Log('Init completed');
	
	Result := true;
end;


procedure CurUninstallStepChanged (CurUninstallStep: TUninstallStep);
var
  mres : integer;
begin
  case CurUninstallStep of
    usPostUninstall:
      begin
        mres := MsgBox('Do you want to delete saved files?', mbConfirmation, MB_YESNO or MB_DEFBUTTON2)
        if mres = IDYES then
          DelTree(ExpandConstant('{localappdata}\{#AppDataDir}'), True, True, True);
      end;  
  end;
end;
