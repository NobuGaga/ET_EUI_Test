@Echo off

@REM 当前脚本所在文件夹路径
set CurrentPath=C:\GitHub\ET_EUI_Test\UWA_Official_Video\Lesson6_7

@REM ET_EUI_Test 目录所在路径
set ETEUITestPath=C:\GitHub\ET_EUI_Test

@REM 需要操作的相对路径
set ETFrameworkPath=%ETEUITestPath%\ET_framework\ET

@REM 客户端数据方法相对路径
set UnityModelPath=Unity\Codes\Model
set UnityHotfixPath=Unity\Codes\Hotfix

@REM 客户端视图层路径
set UnityHotfixView=Unity\Codes\HotfixView

@REM 服务度数据方法相对路径
set ServerModelPath=Server\Model
set ServerHotfixPath=Server\Hotfix

@REM ====================================================================================
@REM 修改原有框架代码路径
set UnitPath=Demo\Unit

@REM 客户端代码路径
set UnityModelUnitPath=%UnityModelPath%\%UnitPath%
set UnityHotfixUnitPath=%UnityHotfixPath%\%UnitPath%

set HotfixViewUnitPath=%UnityHotfixView%\%UnitPath%
set UILoginPath=%UnityHotfixView%\Demo\UI\UILogin

@REM 新增测试逻辑代码路径
set LessonCodePath=Demo\Computer

@REM ====================================================================================
@REM 测试代码原路径跟备份路径组装

set UnityHotfixLessonCodePath=%UnityHotfixPath%\%LessonCodePath%
set UnityModelLessonCodePath=%UnityModelPath%\%LessonCodePath%

set ServerHotfixLessonCodePath=%ServerHotfixPath%\%LessonCodePath%
set ServerModelLessonCodePath=%ServerModelPath%\%LessonCodePath%

set ETUnityHotfixLessonCodePath=%ETFrameworkPath%\%UnityHotfixLessonCodePath%
set ETUnityModelLessonCodePath=%ETFrameworkPath%\%UnityModelLessonCodePath%

set ETServerHotfixLessonCodePath=%ETFrameworkPath%\%ServerHotfixLessonCodePath%
set ETServerModelLessonCodePath=%ETFrameworkPath%\%ServerModelLessonCodePath%

@REM ====================================================================================
@REM 脚本文件夹操作

rd /S /Q %ETUnityHotfixLessonCodePath%
rd /S /Q %ETUnityModelLessonCodePath%

rd /S /Q %ETServerHotfixLessonCodePath%
rd /S /Q %ETServerModelLessonCodePath%

xcopy %CurrentPath%\%UnityHotfixLessonCodePath% %ETUnityHotfixLessonCodePath% /E /F /I
xcopy %CurrentPath%\%UnityModelLessonCodePath% %ETUnityModelLessonCodePath% /E /F /I

xcopy %CurrentPath%\%ServerHotfixLessonCodePath% %ETServerHotfixLessonCodePath% /E /F /I
xcopy %CurrentPath%\%ServerModelLessonCodePath% %ETServerModelLessonCodePath% /E /F /I

@REM ====================================================================================
@REM 脚本文件操作

@REM 客户端

echo A | xcopy %CurrentPath%\%UnityModelUnitPath%\Unit.cs %ETFrameworkPath%\%UnityModelUnitPath%\Unit.cs /E /F /I
echo A | xcopy %CurrentPath%\%UnityModelUnitPath%\UnitType.cs %ETFrameworkPath%\%UnityModelUnitPath%\UnitType.cs /E /F /I

echo A | xcopy %CurrentPath%\%UnityHotfixUnitPath%\UnitFactory.cs %ETFrameworkPath%\%UnityHotfixUnitPath%\UnitFactory.cs /E /F /I

echo A | xcopy %CurrentPath%\%HotfixViewUnitPath%\AfterUnitCreate_CreateUnitView.cs %ETFrameworkPath%\%HotfixViewUnitPath%\AfterUnitCreate_CreateUnitView.cs /E /F /I
echo A | xcopy %CurrentPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs %ETFrameworkPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs /E /F /I

@REM 服务端
echo A | xcopy %CurrentPath%\%ServerHotfixPath%\AppStart_Init.cs %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs /E /F /I

@REM TODO 将添加代码引用到工程中, 并编译

@Echo on