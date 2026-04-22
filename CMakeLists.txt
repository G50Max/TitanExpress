cmake_minimum_required(VERSION 3.15)
project(TitanExpress)

# 简单的 CMake 配置 - 主要使用外部工具
enable_language(CSharp)

# 添加所有源文件
file(GLOB_RECURSE CS_SOURCES "*.cs")

add_executable(TitanExpress )

# 设置 .NET 相关属性
set_target_properties(TitanExpress PROPERTIES
    DOTNET_TARGET_FRAMEWORK "net9.0"
)
