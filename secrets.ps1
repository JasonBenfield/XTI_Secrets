Import-Module PowershellForXti -Force

$script:secretsConfig = [PSCustomObject]@{
    RepoOwner = "JasonBenfield"
    RepoName = "XTI_Secrets"
    AppName = "XTI_Secrets"
    AppType = "Package"
    ProjectDir = ""
}

function Secrets-New-XtiIssue {
    param(
        [Parameter(Mandatory, Position=0)]
        [string] $IssueTitle,
        $Labels = @(),
        [string] $Body = "",
        [switch] $Start
    )
    $script:secretsConfig | New-XtiIssue @PsBoundParameters
}

function Secrets-Xti-StartIssue {
    param(
        [Parameter(Position=0)]
        [long]$IssueNumber = 0,
        $IssueBranchTitle = "",
        $AssignTo = ""
    )
    $script:secretsConfig | Xti-StartIssue @PsBoundParameters
}

function Secrets-New-XtiVersion {
    param(
        [Parameter(Position=0)]
        [ValidateSet("major", "minor", "patch")]
        $VersionType = "minor",
        [ValidateSet("Development", "Production", "Staging", "Test")]
        $EnvName = "Production"
    )
    $script:secretsConfig | New-XtiVersion @PsBoundParameters
}

function Secrets-Xti-Merge {
    param(
        [Parameter(Position=0)]
        [string] $CommitMessage
    )
    $script:secretsConfig | Xti-Merge @PsBoundParameters
}

function Secrets-New-XtiPullRequest {
    param(
        [Parameter(Position=0)]
        [string] $CommitMessage
    )
    $script:secretsConfig | New-XtiPullRequest @PsBoundParameters
}

function Secrets-Xti-PostMerge {
    param(
    )
    $script:secretsConfig | Xti-PostMerge @PsBoundParameters
}

function Secrets-Publish {
    param(
        [switch] $Prod
    )
    $script:secretsConfig | Xti-PublishPackage @PsBoundParameters
    if($Prod) {
        $script:secretsConfig | Xti-Merge
    }
}
