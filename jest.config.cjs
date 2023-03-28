module.exports = {
  preset: 'ts-jest',

  transform: {
    '^.+\\.ts?$': 'ts-jest',
    '^.+\\.(js|jsx)$': 'babel-jest'
  },
  testMatch: ['*/__tests__/**/*.ts?(x)', '**/?(*.)+(test).ts?(x)'],

  testTimeout: 5000,
  globals: {
    'ts-jest': {
      babelConfig: true
    }
  },
  modulePaths: ['<rootDir>/build/elevator/']
};