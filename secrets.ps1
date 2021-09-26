Import-Module PowershellForXti -Force

$script:secretsConfig = [PSCustomObject]@{
    RepoOwner = "JasonBenfield"
    RepoName = "XTI_Secrets"
    AppName = "XTI_Secrets"
    AppType = "Package"
}

function Secrets-NewVersion {
    param(
        [Parameter(Position=0)]
        [ValidateSet("major", "minor", "patch")]
        $VersionType = "minor"
    )
    $script:secretsConfig | New-XtiVersion @PsBoundParameters
}

function Secrets-NewIssue {
    param(
        [Parameter(Mandatory, Position=0)]
        [string] $IssueTitle,
        [switch] $Start
    )
    $script:secretsConfig | New-XtiIssue @PsBoundParameters
}

function Secrets-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0
    )
    $script:secretsConfig | Xti-StartIssue @PsBoundParameters
}

function Secrets-CompleteIssue {
    param(
    )
    $script:secretsConfig | Xti-CompleteIssue @PsBoundParameters
}

function Secrets-Publish {
    param(
        [ValidateSet("Development", "Production")]
        $EnvName = "Development"
    )
    $script:secretsConfig | Xti-Publish @PsBoundParameters
}