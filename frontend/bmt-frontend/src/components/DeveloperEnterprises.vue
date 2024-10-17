<template>

  <head>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css"
      integrity="sha512-iecdLmaskl7CVkqkXNQ/ZH/XLlvWZOJyj7Yy7tcenmpD1ypASozpmT/E0iPtmFIB46ZmdtAc9eNBvH0H/ZpiBw=="
      crossorigin="anonymous" referrerpolicy="no-referrer" />
  </head>

  <body>
    <div>
      <div class="d-flex justify-content-end"></div>
      <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasNavbar" aria-labelledby="offcanvasNavbarLabel">
        <div class="offcanvas-header">
          <h5 class="offcanvas-title" id="offcanvasNavbarLabel">Opciones</h5>
          <button type="button" class="btn-close" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
          <ul class="navbar-nav justify-content-end">
            <li class="nav-item">
              <a class="nav-link active" aria-current="page" href="#" style="padding: 10px;">Opciones de usuario</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="#" style="padding: 10px;" @click="goToMainPage">Página principal</a>
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

    <div class="main-table">
      <h1 class="table-title">Emprendimientos</h1>
      <div class="grid-container">
        <div class="grid-item header">Nombre</div>
        <div class="grid-item header">Cantidad de productos disponibles</div>
        <div class="grid-item header">Cantidad de empleados</div>
        <div v-for="enterprise in enterprises" :key="enterprise.name" class="grid-row">
          <div class="grid-item">{{ enterprise.name }}</div>
          <div class="grid-item">{{ enterprise.productQuantity }}</div>
          <div class="grid-item">{{ enterprise.employeeQuantity }}</div>
        </div>
      </div>
    </div>
  </body>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

export default {
  data() {
    return {
      enterprises: []
    };
  },
  methods: {
    async getEnterprises() {
      try {
        const response = await axios.get(API_URL + '/Developer/getEnterprises');
        this.enterprises = response.data;
      } catch (error) {
        console.error('Error fetching enterprises:', error);
      }
    },
    goToMainPage() {
      this.$router.push('/developer-home');
    },
  },
  mounted() {
    this.getEnterprises();
  }
}
</script>

<style scoped>
@import 'bootstrap/dist/css/bootstrap.min.css';

body {
  background-color: #D1E4FF;
}

.main-table {
  border-radius: 15px;
  margin: 10%;
  width: 80%;
  background-color: #9FC9FC;
}

.menu {
  font-style: normal;
  font-weight: bold;
  color: #BCD6F3;
}

.grid-row {
  display: contents;
}

.grid-container {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  row-gap: 10px;
  padding: 100px;
  padding-top: 20px;
  font-size: large;
  color: #49454F;
}

.grid-item.header {
  font-weight: bold;
}

.grid-item {
  background-color: #8cb2e0;
  border: 1px solid #8cb2e0;
}

.table-title {
  padding: 50px;
  padding-bottom: 0px;
  color: #49454F;
  font-size: 2.5rem;
  font-weight: 500;
  text-align: center;
}

.d-flex {
  position: fixed;
  top: 0;
  right: 0;
  padding: 10px;
}

.user-icon-container {
  padding: 10px;
  position: fixed;
  top: 0;
  left: 0;
}

.offcanvas-header {
  background-color: #02174B;
}

.offcanvas-title {
  color: #D0EDA0;
}

.offcanvas-body {
  background-color: #BCD6F3;
  margin: 0px;
  padding: 0px;
}

.user-icon-button {
  background-color: #02174B;
  border-radius: 50px;
  padding: 10px;
  cursor: pointer;
}

.btn-close {
  background-color: #BCD6F3;
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
