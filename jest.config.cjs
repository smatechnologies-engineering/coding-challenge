module.exports = {
  preset: 'ts-jest',
  transform: {
    '^.+\\.(t|j)s?$': 'ts-jest',
    '^.+\\.(js|jsx)$': 'babel-jest'
  },
  testMatch: ['**/*.test.js?(x)', '**/*.spec.ts?(x)'],


  testTimeout: 5000,
  modulePaths: ['<rootDir>'],
  testEnvironment: 'node'
};