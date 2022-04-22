@Echo off

set GitHubProjectPath=C:\GitHub\ET_EUI_Test\OtherProgram\FishWorld\Unity
set SVNProjectPath=C:\SVN\fishWorld\ET\Unity

set AssetsPath=Assets
set EditorPath=%AssetsPath%\Editor
set MonoPath=%AssetsPath%\Mono\Battle

set CodesPath=Codes
set HotfixPath=%CodesPath%\Hotfix
set HotfixViewPath=%CodesPath%\HotfixView
set ModelPath=%CodesPath%\Model
set ModelViewPath=%CodesPath%\ModelView

set BattleCodePath=Demo\Battle

set GitHubEditorBattlePath=%GitHubProjectPath%\%EditorPath%\%BattleCodePath%
set GitHubMonoBattlePath=%GitHubProjectPath%\%MonoPath%
set GitHubHotfixBattlePath=%GitHubProjectPath%\%HotfixPath%\%BattleCodePath%
set GitHubHotfixViewBattlePath=%GitHubProjectPath%\%HotfixViewPath%\%BattleCodePath%
set GitHubModelBattlePath=%GitHubProjectPath%\%ModelPath%\%BattleCodePath%
set GitHubModelViewBattlePath=%GitHubProjectPath%\%ModelViewPath%\%BattleCodePath%

rd /S /Q %GitHubEditorBattlePath%
rd /S /Q %GitHubMonoBattlePath%
rd /S /Q %GitHubHotfixBattlePath%
rd /S /Q %GitHubHotfixViewBattlePath%
rd /S /Q %GitHubModelBattlePath%
rd /S /Q %GitHubModelViewBattlePath%

xcopy %SVNProjectPath%\%EditorPath%\%BattleCodePath% %GitHubEditorBattlePath% /E /F /I
xcopy %SVNProjectPath%\%MonoPath% %GitHubMonoBattlePath% /E /F /I
xcopy %SVNProjectPath%\%HotfixPath%\%BattleCodePath% %GitHubHotfixBattlePath% /E /F /I
xcopy %SVNProjectPath%\%HotfixViewPath%\%BattleCodePath% %GitHubHotfixViewBattlePath% /E /F /I
xcopy %SVNProjectPath%\%ModelPath%\%BattleCodePath% %GitHubModelBattlePath% /E /F /I
xcopy %SVNProjectPath%\%ModelViewPath%\%BattleCodePath% %GitHubModelViewBattlePath% /E /F /I

@Echo on