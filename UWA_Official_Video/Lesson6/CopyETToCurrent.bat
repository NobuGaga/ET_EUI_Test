@Echo off

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

rd /S /Q %UnityHotfixLessonCodePath%
rd /S /Q %UnityModelLessonCodePath%

rd /S /Q %ServerHotfixLessonCodePath%
rd /S /Q %ServerModelLessonCodePath%

xcopy %ETUnityHotfixLessonCodePath% %UnityHotfixLessonCodePath% /E /F /I
xcopy %ETUnityModelLessonCodePath% %UnityModelLessonCodePath% /E /F /I
xcopy %ETServerHotfixLessonCodePath% %ServerHotfixLessonCodePath% /E /F /I
xcopy %ETServerModelLessonCodePath% %ServerModelLessonCodePath% /E /F /I

rd /S /Q %ETUnityHotfixLessonCodePath%
rd /S /Q %ETUnityModelLessonCodePath%

rd /S /Q %ETServerHotfixLessonCodePath%
rd /S /Q %ETServerModelLessonCodePath%

echo f | xcopy %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs %ServerHotfixPath%\AppStart_Init.cs /E /F /I

set AppStartInitFinish_CreateLoginUIPath=Unity\Codes\HotfixView\Demo\UI\UILogin\AppStartInitFinish_CreateLoginUI.cs

echo f | xcopy %ETFrameworkPath%\%AppStartInitFinish_CreateLoginUIPath% %AppStartInitFinish_CreateLoginUIPath% /E /F /I

@Echo on