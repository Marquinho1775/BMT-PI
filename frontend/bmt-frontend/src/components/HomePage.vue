<template>
  <head>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH" crossorigin="anonymous">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" integrity="sha512-ChzD5N+nKyoRSdeUVM4u/wJ6p9DZASogZ30E+5VqC/94wcjx3v+iIv2QO+j25a9E+9Hj+MEgQ8NNQWidgetr4t+iIv2QO+j25a9E+9Hj+MEgQ8NNQWidgetr4t+" crossorigin="anonymous" referrerpolicy="no-referrer" />
  </head>
  <body>
    <div class="container-fluid d-flex flex-column align-items-center justify-content-center" style="height: 100vh;">
      <div>
        <h2 class="title">Bienvenido a Business Tracker</h2>
      </div>
    </div>
    <div>
      <div class="d-flex justify-content-end">
        <button class="btn btn-primary" @click="handleLogin">Iniciar Sesión</button>
        <button class="btn btn-secondary" @click="handleRegister">Registrarse</button>
      </div>
      <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasNavbar" aria-labelledby="offcanvasNavbarLabel">
      <div class="offcanvas-header" style="background-color: #02174B;">  
  
        <h5 class="offcanvas-title" id="offcanvasNavbarLabel">Menú</h5>
        <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close" style="background-color: #BCD6F3;"></button>
        </div>
        <div class="offcanvas-body" style="background-color: #BCD6F3;">
          <ul class="navbar-nav  
            justify-content-end">
          </ul>
        </div>
      </div>
  
      <div class="user-icon-container">
        <button class="user-icon-button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar" aria-controls="offcanvasNavbar">
          <i class="fa-user">Menú</i>
        </button>
      </div>
    </div>
  </body>
</template>
  
<script>
import Bootstrap from 'bootstrap/dist/js/bootstrap.bundle.min.js';
export default {
  methods: {
    mounted() {
      Bootstrap();
    },
    handleLogin() {
      console.log('Navigating to login view');
      this.$router.push('/login');
    },
    handleRegister() {
      console.log('Navigating to register view');
      this.$router.push('/register');
    }
  }
}
</script>
  
<style scoped>
  @import 'bootstrap/dist/css/bootstrap.min.css';
  .logo {
    display: none;
  }
  .nav-tabs .nav-link {
    background-color: #BCD6F3;
    border: none;
    padding: 10px 20px;
    cursor: pointer;
    font-weight: bold;
  }
  .title {
    background-color: #D0EDA0;
    color: #02174B;
    padding: 50px;
    border-radius: 100px;
  }
  .nav-tabs .nav-link.active {
    background-color: #8FA3BE;
    color: #91AC65;
  }
  .d-flex {
    position: fixed;
    top: 0;
    right: 0;
    padding: 10px;
  }
  .user-icon-container {
    padding: 15px;
    position: fixed;
    top: 0;
    left: 0;
    padding: 10px;
  }
  .fa-user {
    color: #BCD6F3;
  }
  .offcanvas-title {
    color: #D0EDA0;
  }
  .user-icon-button {
    background-color: #02174B;
    border-radius: 50px;
    padding: 10px;
    cursor: pointer;
  }
  .btn-primary {
    background-color: #39517B; 
  }
  .btn-secondary {
    background-color: #D0EDA0;
    color: #02174B;
  }
  .btn-primary:hover {
    background-color: #02174B;
  }
  .btn-secondary:hover {
    background-color: #384D13;
    color: #D0EDA0;
  }
  .nav-tabs .nav-link {
  transition: none;
  background-color: #BCD6F3;
  border: none;
  padding: 10px 20px;
  cursor: pointer;
  font-weight: bold;
  }
  .nav-link:hover {
    transition: none;
    background-color: #9ab0c9;
    border: none;
    margin: none;
    padding: none;
    cursor: pointer;
    font-weight: bold;
  }
</style>
  