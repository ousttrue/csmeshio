project "meshioTest"
--language "C"
--language "C++"
language "C#"
--kind "StaticLib"
kind "SharedLib"
--kind "ConsoleApp"
--kind "WindowedApp"

files {
    "*.cs",
}
defines {
}
includedirs {
}
libdirs {
    "C:/Program Files (x86)/NUnit 2.6/bin/framework",
}
links {
    --"System",
    --"System.Drawing",
    --"System.Windows.Forms",
    "nunit.framework",
    "meshio",
}

