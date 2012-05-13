-- A solution contains projects, and defines the available configurations
solution "meshio#"
configurations { "Release", "Debug" }
configuration "windows*"
do
end

configuration "Debug"
do
  flags { "Symbols" }
  targetdir "debug"
end

configuration "Release"
do
  flags { "Optimize" }
  targetdir "release"
end

configuration {}

include "meshio"
include "tests"

