import { Octokit } from "octokit";

export class IssueSyncer {
    public static getIssueNumberByTitle(octokit: Octokit, owner: string, repo: string, issue_title: string): Promise<number> {
        // retrieve issue from target repo by title
        return octokit.request('GET /repos/{owner}/{repo}/issues', {
            owner: owner,
            repo: repo,
            title: issue_title
        }).then((response) => {
            return response.data[0].number;
        });  
    }
}