$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

dotnet --version
node --version
npm --version

dotnet restore GameNet.slnx
dotnet build GameNet.slnx --configuration Release --no-restore
dotnet test GameNet.slnx --configuration Release --no-build --no-restore

Push-Location "src/Dashboard"
try {
    npm install
    npm run typecheck
    npm run build
}
finally {
    Pop-Location
}
