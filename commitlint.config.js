// https://commitlint.js.org
//
// Commit message format:
// <Jira work item key> <type>: <subject>
// Example: W18ADATE-123 feat: add login form

const JIRA_PROJECT_KEY = 'W18ADATE';
const COMMIT_TYPES = [
  'feat',
  'fix',
  'refactor',
  'test',
  'ops',
  'style',
  'docs',
  'chore',
];

module.exports = {
  parserPreset: {
    parserOpts: {
      headerPattern: new RegExp(
        `^${JIRA_PROJECT_KEY}-\\d+\\s(${COMMIT_TYPES.join('|')}):\\s(.+)$`
      ),
      headerCorrespondence: ['type', 'subject'],
    },
  },
  rules: {
    'type-enum': [2, 'always', COMMIT_TYPES],
    'type-empty': [2, 'never'],
    'subject-empty': [2, 'never'],
  },
};

// test commit linter
