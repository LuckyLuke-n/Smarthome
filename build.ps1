param( [string]$pushToDockerHub = "y" )

# Set-Location .\src-docker\PantryManagerWeb.API
Set-Location C:\Sources\LuckyLuke-n\Smarthome

# tag
try
{
    $tag = $(git describe --tags).Split("-")[0]
}
catch
{
    $tag = "untagged"
}

# branch name
$branch = $(git rev-parse --abbrev-ref HEAD)

# create a string for the commit counter
$commitCount = [int]$(git rev-list --all --count)
if ( $commitCount -lt 10 )
{
    $commitCountString = "000$($commitCount)"
}
elseif ( $commitCount -gt 9 -and $commitCount -lt 100 )
{
    $commitCountString = "00$($commitCount)"
}
elseif ( $commitCount -gt 99 -and $commitCount -lt 1000 )
{
    $commitCountString = "0$($commitCount)"
}
else
{
    $commitCountString  = $($commitCount)
}

# concatenate the full version string
# e.g.: 1.0.0-feature0001
if ( $branch -eq "master" )
{
    $fullVersionString = "$($tag)"
}
else
{
    $fullVersionString = "$($tag)-$($branch.Split("/")[0])$($commitCountString)"
}

# build the container
docker build --platform=linux/arm64 -t luckyluke4411/ambientcollector:$fullVersionString -f src-docker\Smarthome.AmbientCollector.Api\Dockerfile .

# push if requested
if ( $pushToDockerHub -eq "y" )
{
    Write-Output("Pushing to DockerHub...")
    docker push luckyluke4411/ambientcollector:$fullVersionString
    Write-Output("Done.")
}
else
{
    Write-Output("Pushing to DockerHub skipped. Done.")
}
