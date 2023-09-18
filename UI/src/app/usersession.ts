class UserSession {
  user: User = new User();
  isLogged: boolean = false;

  login(user: User) {
    this.user = user;
    this.isLogged = true;
  }
}
