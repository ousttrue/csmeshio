project "meshioViewer"
--language "C"
--language "C++"
language "C#"
--kind "StaticLib"
--kind "SharedLib"
--kind "ConsoleApp"
kind "WindowedApp"

files {
    "*.cs",
}
defines {
}
includedirs {
}
libdirs {
    "C:/Program Files (x86)/SlimDX SDK (January 2012)/Bin/net40/x86",
}
links {
    "System",
    "System.Drawing",
    "System.Windows.Forms",
    "SlimDX",
    "meshio",
}

