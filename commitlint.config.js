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

// https://commitlint.js.org
// <Jira work item key> <type>: <message>
module.exports = {
  rules: {
    'header-pattern': [
      2,
      'always',
      new RegExp(
        `^${JIRA_PROJECT_KEY}-\\d+\\s(${COMMIT_TYPES.join('|')}):\\s.{1,}$`
      ),
    ],
  },
};

// test commit messages linter
