@Echo off

@REM 当前脚本所在文件夹路径
set CurrentPath=C:\GitHub\ET_EUI_Test\UWA_Official_Video\Lesson6_7

@REM ET_EUI_Test 目录所在路径
set ETEUITestPath=C:\GitHub\ET_EUI_Test

@REM 需要操作的相对路径
set ETFrameworkPath=%ETEUITestPath%\ET_framework\ET

set UnityHotfixPath=Unity\Codes\Hotfix
set UnityModelPath=Unity\Codes\Model

set ServerHotfixPath=Server\Hotfix
set ServerModelPath=Server\Model

set LessonCodePath=Demo\Computer

set UnityHotfixLessonCodePath=%UnityHotfixPath%\%LessonCodePath%
set UnityModelLessonCodePath=%UnityModelPath%\%LessonCodePath%

set ServerHotfixLessonCodePath=%ServerHotfixPath%\%LessonCodePath%
set ServerModelLessonCodePath=%ServerModelPath%\%LessonCodePath%

set ETUnityHotfixLessonCodePath=%ETFrameworkPath%\%UnityHotfixLessonCodePath%
set ETUnityModelLessonCodePath=%ETFrameworkPath%\%UnityModelLessonCodePath%

set ETServerHotfixLessonCodePath=%ETFrameworkPath%\%ServerHotfixLessonCodePath%
set ETServerModelLessonCodePath=%ETFrameworkPath%\%ServerModelLessonCodePath%

rd /S /Q %ETUnityHotfixLessonCodePath%
rd /S /Q %ETUnityModelLessonCodePath%

rd /S /Q %ETServerHotfixLessonCodePath%
rd /S /Q %ETServerModelLessonCodePath%

xcopy %CurrentPath%\%UnityHotfixLessonCodePath% %ETUnityHotfixLessonCodePath% /E /F /I
xcopy %CurrentPath%\%UnityModelLessonCodePath% %ETUnityModelLessonCodePath% /E /F /I
xcopy %CurrentPath%\%ServerHotfixLessonCodePath% %ETServerHotfixLessonCodePath% /E /F /I
xcopy %CurrentPath%\%ServerModelLessonCodePath% %ETServerModelLessonCodePath% /E /F /I

echo A | xcopy %CurrentPath%\%ServerHotfixPath%\AppStart_Init.cs %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs /E /F /I

set UILoginPath=Unity\Codes\HotfixView\Demo\UI\UILogin

echo A | xcopy %CurrentPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs %ETFrameworkPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs /E /F /I

@Echo on