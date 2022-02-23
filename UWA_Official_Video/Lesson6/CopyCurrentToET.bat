REM @Echo off

set ETFrameworkPath=..\..\ET_framework\ET

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

xcopy %UnityHotfixLessonCodePath% %ETUnityHotfixLessonCodePath% /E /F /I
xcopy %UnityModelLessonCodePath% %ETUnityModelLessonCodePath% /E /F /I
xcopy %ServerHotfixLessonCodePath% %ETServerHotfixLessonCodePath% /E /F /I
xcopy %ServerModelLessonCodePath% %ETServerModelLessonCodePath% /E /F /I

rd /S /Q %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs

echo f | xcopy %ServerHotfixPath%\AppStart_Init.cs %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs /E /F /I

set AppStartInitFinish_CreateLoginUIPath=Unity\Codes\HotfixView\Demo\UI\UILogin\AppStartInitFinish_CreateLoginUI.cs

rd /S /Q %ETFrameworkPath%\%AppStartInitFinish_CreateLoginUIPath%

echo f | xcopy %AppStartInitFinish_CreateLoginUIPath% %ETFrameworkPath%\%AppStartInitFinish_CreateLoginUIPath% /E /F /I

REM @Echo on