@Echo off

@REM 当前脚本所在文件夹路径
set CurrentPath=C:\GitHub\ET_EUI_Test\UWA_Official_Video\Lesson6_13

@REM ET_EUI_Test 目录所在路径
set ETEUITestPath=C:\GitHub\ET_EUI_Test

@REM submodule 路径
set ETFrameworkPath=%ETEUITestPath%\ET_framework\ET

@REM 配置表路径
set ExcelPath=Excel
set ETExcelPath=%ETFrameworkPath%\%ExcelPath%
set CurrentExcelPath=%CurrentPath%\%ExcelPath%

@REM Proto 路径
set ProtoPath=Proto
set ETProtoPath=%ETFrameworkPath%\%ProtoPath%
set CurrentProtoPath=%CurrentPath%\%ProtoPath%

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
set EntityPath=Core\Entity
set DemoPath=Demo
set DemoLoginPath=%DemoPath%\Login
set UnitPath=%DemoPath%\Unit
set ScenePath=%DemoPath%\Scene

@REM 客户端代码路径
set UnityEntityPath=%UnityModelPath%\%EntityPath%

set UnityModelDemoPath=%UnityModelPath%\%DemoPath%
set UnityModelUnitPath=%UnityModelPath%\%UnitPath%

set UnityHotfixLoginPath=%UnityHotfixPath%\%DemoLoginPath%
set UnityHotfixUnitPath=%UnityHotfixPath%\%UnitPath%

set HotfixViewDemoPath=%UnityHotfixView%\%DemoPath%
set HotfixViewUnitPath=%UnityHotfixView%\%UnitPath%

set UnityScenePath=%UnityHotfixPath%\%ScenePath%
set UILoginPath=%UnityHotfixView%\%DemoPath%\UI\UILogin

@REM 服务器代码路径
set ServerDemoLoginPaht=%ServerHotfixPath%\%DemoLoginPath%

@REM 新增测试逻辑代码路径
set LessonComputerPath=%DemoPath%\Computer
set LessonUnitConfigPartialPath=%UnityModelPath%\Generate\ConfigPartial

@REM ====================================================================================
@REM 测试代码原路径跟备份路径组装

set UnityHotfixLessonCodePath=%UnityHotfixPath%\%LessonComputerPath%
set UnityModelLessonCodePath=%UnityModelPath%\%LessonComputerPath%

set ServerHotfixLessonCodePath=%ServerHotfixPath%\%LessonComputerPath%
set ServerModelLessonCodePath=%ServerModelPath%\%LessonComputerPath%

set ETUnityHotfixLessonCodePath=%ETFrameworkPath%\%UnityHotfixLessonCodePath%
set ETUnityModelLessonCodePath=%ETFrameworkPath%\%UnityModelLessonCodePath%

set ETServerHotfixLessonCodePath=%ETFrameworkPath%\%ServerHotfixLessonCodePath%
set ETServerModelLessonCodePath=%ETFrameworkPath%\%ServerModelLessonCodePath%

set ETServerHotfixDemoLoginPath=%ETFrameworkPath%\%ServerDemoLoginPaht%

@REM ====================================================================================
@REM 脚本文件夹操作

rd /S /Q %CurrentPath%\%UnityHotfixLessonCodePath%
rd /S /Q %CurrentPath%\%UnityModelLessonCodePath%

rd /S /Q %CurrentPath%\%ServerHotfixLessonCodePath%
rd /S /Q %CurrentPath%\%ServerModelLessonCodePath%

rd /S /Q %CurrentPath%\%ServerDemoLoginPaht%

xcopy %ETUnityHotfixLessonCodePath% %CurrentPath%\%UnityHotfixLessonCodePath% /E /F /I
xcopy %ETUnityModelLessonCodePath% %CurrentPath%\%UnityModelLessonCodePath% /E /F /I

xcopy %ETServerHotfixLessonCodePath% %CurrentPath%\%ServerHotfixLessonCodePath% /E /F /I
xcopy %ETServerModelLessonCodePath% %CurrentPath%\%ServerModelLessonCodePath% /E /F /I

xcopy %ETServerHotfixDemoLoginPath% %CurrentPath%\%ServerDemoLoginPaht% /E /F /I

rd /S /Q %ETUnityHotfixLessonCodePath%
rd /S /Q %ETUnityModelLessonCodePath%

rd /S /Q %ETServerHotfixLessonCodePath%
rd /S /Q %ETServerModelLessonCodePath%

rd /S /Q %ETServerHotfixDemoLoginPath%

@REM ====================================================================================
@REM 脚本文件操作

@REM 配置表

echo A | xcopy %ETExcelPath%\StartSceneConfig.xlsx %CurrentExcelPath%\StartSceneConfig.xlsx /E /F /I
echo A | xcopy %ETExcelPath%\UnitConfig.xlsx %CurrentExcelPath%\UnitConfig.xlsx /E /F /I

@REM Proto

echo A | xcopy %ETProtoPath%\OuterMessage.proto %CurrentProtoPath%\OuterMessage.proto /E /F /I

@REM 客户端

echo A | xcopy %ETFrameworkPath%\%UnityEntityPath%\SceneType.cs %CurrentPath%\%UnityEntityPath%\SceneType.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityModelDemoPath%\EventType.cs %CurrentPath%\%UnityModelDemoPath%\EventType.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityModelUnitPath%\Unit.cs %CurrentPath%\%UnityModelUnitPath%\Unit.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityModelUnitPath%\UnitType.cs %CurrentPath%\%UnityModelUnitPath%\UnitType.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%LessonUnitConfigPartialPath%\UnitConfigPartial.cs %CurrentPath%\%LessonUnitConfigPartialPath%\UnitConfigPartial.cs /E /F /I

echo A | xcopy %ETFrameworkPath%\%UnityHotfixLoginPath%\LoginHelper.cs %CurrentPath%\%UnityHotfixLoginPath%\LoginHelper.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityHotfixLoginPath%\R2C_SayGoodByeHandler.cs %CurrentPath%\%UnityHotfixLoginPath%\R2C_SayGoodByeHandler.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityHotfixUnitPath%\UnitFactory.cs %CurrentPath%\%UnityHotfixUnitPath%\UnitFactory.cs /E /F /I

echo A | xcopy %ETFrameworkPath%\%HotfixViewDemoPath%\EventType.cs %CurrentPath%\%HotfixViewDemoPath%\EventType.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%HotfixViewUnitPath%\AfterUnitCreate_CreateUnitView.cs %CurrentPath%\%HotfixViewUnitPath%\AfterUnitCreate_CreateUnitView.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UnityScenePath%\SceneFactory.cs %CurrentPath%\%UnityScenePath%\SceneFactory.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs %CurrentPath%\%UILoginPath%\AppStartInitFinish_CreateLoginUI.cs /E /F /I
echo A | xcopy %ETFrameworkPath%\%UILoginPath%\UILoginComponentSystem.cs %CurrentPath%\%UILoginPath%\UILoginComponentSystem.cs /E /F /I


@REM 服务端

echo A | xcopy %ETFrameworkPath%\%ServerHotfixPath%\AppStart_Init.cs %CurrentPath%\%ServerHotfixPath%\AppStart_Init.cs /E /F /I

@REM TODO 还原修改文件的操作, 并清理工程引用

@Echo on