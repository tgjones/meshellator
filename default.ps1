# Thanks to AutoMapper for the inspiration for this build file.

$global:config = 'Release'
$framework = '4.0'
$product_name = "Meshellator"

properties {
    $base_dir = resolve-path .
    $source_dir = "$base_dir\src"
    $build_dir = "$base_dir\build"
    $package_dir = "$base_dir\package"
    $nunit_dir = "$base_dir\src\packages\NUnit.2.5.10.11092\tools"
    $nuget_dir = "$base_dir\src\packages\NuGet.CommandLine.1.5.21005.9019\tools"
}

task Default -depends Clean, Compile, Test, Package

task Clean {
    delete_directory "$package_dir"
}

task Compile -depends Clean {
    exec { msbuild /t:Clean /t:Build /p:Configuration=$config /v:q /nologo $source_dir\$product_name.sln }
}

task Test -depends Compile {
    $tests_dir = "$source_dir/$product_name.Tests/bin/$config"
    exec { & $nunit_dir\nunit-console-x86.exe $tests_dir/$product_name.Tests.dll /nologo /nodots /xml=$tests_dir/$product_name.Tests.TestResults.xml }
}

task Package -depends Test {
    create_directory "$package_dir"
     
    # Copy NuSpec template files to package dir
    cp "$build_dir\$product_name.nuspec" "$package_dir"
    cp "$build_dir\LICENSE.txt" "$package_dir"

    # Copy binary files to package dir
    copy_files "$source_dir\Meshellator\bin\$config" "$package_dir\lib\NET40" "Meshellator.dll","Meshellator.pdb"
    copy_files "$source_dir\Meshellator.Silverlight\bin\$config" "$package_dir\lib\SL4" "Meshellator.Silverlight.dll","Meshellator.Silverlight.pdb"

    # Copy source files to package dir
    copy_files "$source_dir\$product_name" "$package_dir\src\$product_name" "*.cs"

    # Get the version number of main DLL
    $full_version = [Reflection.Assembly]::LoadFile("$source_dir\$product_name\bin\$config\$product_name.dll").GetName().Version
    $version = $full_version.Major.ToString() + "." + $full_version.Minor.ToString() + "." + $full_version.Build.ToString()

    # Build the NuGet package
    exec { & $nuget_dir\NuGet.exe pack -Symbols -Version "$version" -OutputDirectory "$package_dir" "$package_dir\$product_name.nuspec" }

    # Push NuGet package to nuget.org
    exec { & $nuget_dir\NuGet.exe push "$package_dir\$product_name.$version.nupkg" }
}

# Helper functions

function global:delete_directory($directory_name) {
    rd $directory_name -recurse -force -ErrorAction SilentlyContinue | out-null
}

function global:create_directory($directory_name)
{
    mkdir $directory_name -ErrorAction SilentlyContinue | out-null
}

function global:copy_files($source, $destination, $include = @(), $exclude = @()) {
    create_directory $destination
    
    $items = Get-ChildItem $source -Recurse -Include $include -Exclude $exclude
    foreach ($item in $items) {
        $dir = $item.DirectoryName.Replace($source,$destination)
        $target = $item.FullName.Replace($source,$destination)

        if (!(test-path($dir))) {
            create_directory $dir
        }
        
        if (!(test-path($target))) {
            cp -path $item.FullName -destination $target
        }
    }
}