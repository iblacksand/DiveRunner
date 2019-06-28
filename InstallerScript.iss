; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "DiveRunner"
#define MyAppVersion "0.1"
#define MyAppPublisher "iblacksand"
#define MyAppURL "https://iblacksand.github.io/"
#define MyAppExeName "DiveRunner.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{A43D2FD2-F89E-4CBA-B429-77C89E22A971}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
; The [Icons] "quicklaunchicon" entry uses {userappdata} but its [Tasks] entry has a proper IsAdminInstallMode Check.
UsedUserAreasWarning=no
; Remove the following line to run in administrative install mode (install for all users.)
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=.\Installer
OutputBaseFilename=DiveRunnerSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
Source: ".\DiveRunner\bin\Debug\DiveRunner.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\AnnouncementTemplate.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\CombinedTemplateLandscape.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\CombinedTemplatePortrait.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\divelist.csv"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\DivelistTemplate.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\DiveScoreTemplate.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Fastenshtein.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Fastenshtein.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Newtonsoft.Json.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\ResultsTemplate.tex"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\SampleCore.json"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\SampleCoreWithScores.json"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Xceed.Wpf.AvalonDock.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Xceed.Wpf.AvalonDock.Themes.Aero.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Xceed.Wpf.AvalonDock.Themes.Metro.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Xceed.Wpf.AvalonDock.Themes.VS2010.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: ".\DiveRunner\bin\Debug\Xceed.Wpf.Toolkit.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

