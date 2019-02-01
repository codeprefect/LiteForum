
describe('Landing Page', () => {
  // load http://localhost:4200 before each test
  beforeEach(() => cy.visit('/'));

  it('should love homepage', () => {
    cy.get('a').contains('LiteForum');
  });
});
