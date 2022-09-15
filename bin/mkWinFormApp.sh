#!/bin/sh

me=$(basename $0)

dohelp() {
	cat << DOHELP
${me} [-h|--help] : this text
${me} [OPTIONS] application_name
OPTIONS:
	--dir dirname : directory name where to create the new
		application directory, default current directory
	--net netversion : exemples net472, net48 (default value: net48)
DOHELP
	exit 0
}

appname=
dirname=.
net=net472

create_app() {
	odir=${dirname}/${appname}
	projectf=${odir}/${appname}.fsproj
	sourcef=${odir}/${appname}.fs

	mkdir -p ${odir} || exit 1

	cat > ${projectf} << APP_PRJ
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>

    <WarnOn>3390;\$(WarnOn)</WarnOn>

    <TargetFramework>${net}</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="${appname}.fs" />
  </ItemGroup>

  <ItemGroup>
    <Reference Include="System.Drawing.Design" />
    <Reference Include="System.Reflection.Context" />
    <Reference Include="System.Windows" Condition="'\$(TargetFramework)' == '${net}'" />
    <Reference Include="System.Windows.Forms" />
  </ItemGroup>

</Project>
APP_PRJ

	cat > ${sourcef} << APP_SRC
(*
    Project:    ${appname}
    File name:  ${appname}.fs
    User:       bernard
    Date:       $(date "+%Y-%m-%d")

    The MIT License (MIT)

    Copyright (c) $(date "+%Y") Bernard TATIN

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.

 *)


open System
open System.Drawing
open System.Windows.Forms

module main =

    [<EntryPoint>]
    let main argv =
        let form = new Form(Text="FImage")
        Application.Run form
        0

APP_SRC
}

[ $# -eq 0 ] && dohelp

while [ $# -ne 0 ]
do
	case $1 in
		'-h'|'--help')
			dohelp
			;;
		'--dir')
			shift
			[ $# -eq 0 ] && dohelp
			dirname=$1
			;;
		'--net')
			shift
			[ $# -eq 0 ] && dohelp
			net=$1
			;;
		*)
			appname=$1
			break
			;;
	esac
	shift
done

create_app
