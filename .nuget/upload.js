const {
    exec
} = require('child_process')
const fs = require('fs')
const iconv = require('iconv-lite')
const co = require('co')

const readFilePromise = (fileName, encoding) => {
    return new Promise((resolve, reject) => {
        fs.readFile(fileName, (err, data) => {
            if (err) {
                reject(err)
            } else {
                var str = iconv.decode(data, encoding)
                resolve(str)
            }
        })
    })
}

const execPromise = (cmdStr) => {
    return new Promise((resolve, reject) => {
        exec(cmdStr, {
            encoding: 'buffer'
        }, (error, stdout) => {
            if (error) {
                reject(error)
            } else {
                resolve(iconv.decode(stdout, 'cp936'))
            }
        })
    })
}

const projectName = 'LibTool'
const nugetPath = process.argv.length > 2 ? process.argv[2] : ''
const csprojPath = process.argv.length > 3 ? process.argv[3] : `../${projectName}/`
const binPath = process.argv.length > 4 ? process.argv[4] : `../${projectName}/bin/Release/`
const cmdList1 = [`dotnet pack ${csprojPath}${projectName}.csproj -c Release --include-source --include-symbols`]
const cmdList2 = [
    `${nugetPath}nuget.exe SetApiKey oy2diiweynyetruzfycqaqvery7ahekulmjjcfuo7yvk4y -Source https://www.nuget.org/packages`,
    `${nugetPath}nuget.exe SetApiKey oy2diiweynyetruzfycqaqvery7ahekulmjjcfuo7yvk4y -s https://api.nuget.org/v3/index.json`,
]

readFilePromise(`../${projectName}/${projectName}.csproj`, 'gb2312')
    .then(str => {
        const version = /Version>([^<]+)(?:<)/g.exec(str)[1]
        return version
    })
    .then(version => {
        const cmdList3 = [
            `${nugetPath}nuget.exe push ${binPath}Feebool.${projectName}.${version}.nupkg oy2diiweynyetruzfycqaqvery7ahekulmjjcfuo7yvk4y -Source https://www.nuget.org/packages`,
            `${nugetPath}nuget.exe push ${binPath}Feebool.${projectName}.${version}.symbols.nupkg oy2diiweynyetruzfycqaqvery7ahekulmjjcfuo7yvk4y -source https://nuget.smbsrc.net`,
            `dotnet nuget push ${binPath}Feebool.${projectName}.${version}.nupkg oy2diiweynyetruzfycqaqvery7ahekulmjjcfuo7yvk4y -s https://api.nuget.org/v3/index.json`
        ]
        const cmdList = [...cmdList1, ...cmdList2, ...cmdList3]

        co(function* () {
            for (const cmd of cmdList) {
                var response = yield execPromise(cmd)
                console.log(response);
            }
        });
    })