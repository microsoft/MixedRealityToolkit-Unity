"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.IssueSyncer = void 0;
class IssueSyncer {
    static getIssueNumberByTitle(octokit, owner, repo, issue_title) {
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
exports.IssueSyncer = IssueSyncer;
