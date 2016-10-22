:: This is a MODIFIED npm.cmd to try using the locally installed version...

@ECHO OFF

SETLOCAL

:: Lets establish node.. hopfully the one next to this file..
SET "NODE_EXE=%~dp0\node.exe"
IF NOT EXIST "%NODE_EXE%" (
  SET "NODE_EXE=node"
)

:: Now where is npm??  I hope it is here relative to the CWD
:: If not, then lets just use the system 'npm' and hope for the best.
SET "NPM_CLI_JS=.\node_modules\npm\bin\npm-cli.js"
IF EXIST "%NPM_CLI_JS%" (
  "%NODE_EXE%" "%NPM_CLI_JS%" %*
) ELSE (
  npm %*
)



