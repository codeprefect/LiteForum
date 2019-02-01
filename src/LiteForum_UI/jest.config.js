module.exports = {
  globals: {
    "ts-jest": {
      "allowSyntheticDefaultImports": true
    }
  },
  testMatch: [
    "<rootDir>/src/**/__tests__/**/?(*.)(spec|test)ts",
    "<rootDir>/src/**/?(*.)(spec|test).ts"
  ],
  testEnvironment: "jsdom"
}
