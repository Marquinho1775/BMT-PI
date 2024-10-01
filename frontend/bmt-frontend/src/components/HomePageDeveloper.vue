<template>

  <head>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"
      integrity="sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw=="
      crossorigin="anonymous" referrerpolicy="no-referrer" />
  </head>

  <body>
    <div class="container-fluid d-flex flex-column align-items-center justify-content-center" style="height: 100vh;">
      <div>
        <h2 class="title">Bienvenido a Business Tracker</h2>
      </div>
    </div>
    <div>
      <div class="d-flex justify-content-end">
      </div>
      <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasNavbar" aria-labelledby="offcanvasNavbarLabel">
        <div class="offcanvas-header" style="background-color: #02174B;">

          <h5 class="offcanvas-title" id="offcanvasNavbarLabel">Opciones</h5>
          <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"
            style="background-color: #BCD6F3;"></button>
        </div>
        <div class="offcanvas-body" style="background-color: #BCD6F3; margin: 0px; padding: 0px">
          <ul class="navbar-nav  
            justify-content-end">
            <li class="nav-item">
              <a class="nav-link active" aria-current="page" href="#" style="padding: 10px;">Opciones de usuario</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#" style="padding: 10px;" @click="goToEnterprises">Emprendimientos</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#" style="padding: 10px;" @click="goToProducts">Productos</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#" style="padding: 10px;" @click="goToUsers">Usuarios</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#" style="padding: 10px;">Cerrar sesión</a>
            </li>
          </ul>
        </div>
      </div>
      <div class="user-icon-container">
        <button class="user-icon-button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasNavbar"
          aria-controls="offcanvasNavbar">
          <i class="menu">Menú</i>
        </button>
      </div>
    </div>
  </body>
</template>

<script>
import Bootstrap from 'bootstrap/dist/js/bootstrap.bundle.min.js';
export default {
  data() {
    return {
    };
  },
  methods: {
    mounted() {
      Bootstrap();
    },
    handleLogout() {
      console.log('Navigating to logout view');
    },
    goToEnterprises() {
      this.$router.push('/developer-enterprises');
    },
    goToProducts() {
      this.$router.push('/developer-products');
    },
    goToUsers() {
      this.$router.push('/developer-users');
    },
  }
}
</script>

<style scoped>
@import 'bootstrap/dist/css/bootstrap.min.css';

.logo {
  display: none;
}

.menu {
  font-style: normal;
  font-weight: bold;
  color: #BCD6F3;
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
  position: fixed;
  top: 0;
  left: 0;
  padding: 10px;
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
</style>