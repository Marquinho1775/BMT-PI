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
      <h1 class="table-title">Usuarios</h1>
      <div class="grid-container">
        <div class="grid-item header">Nombre</div>
        <div class="grid-item header">Correo</div>
        <div class="grid-item header">Cédula</div>
        <div class="grid-item header">Empresas asociadas</div>
        <div v-for="user in users" :key="user.name" class="grid-row">
          <div class="grid-item">{{ user.name }}</div>
          <div class="grid-item">{{ user.email }}</div>
          <div class="grid-item">{{ user.identification }}</div>
          <div class="grid-item">
            <span class="d-inline-block" tabindex="0" data-bs-toggle="popover" data-bs-trigger="hover focus"
              :data-bs-html="true" :data-bs-content="user.associatedCompanies.join('<br>')">
              <button class="btn custom-popover-btn" type="button">...</button>
            </span>
          </div>
        </div>
      </div>
    </div>
  </body>
</template>

<script>
import axios from 'axios';
import { API_URL } from '@/main.js';
import { Popover } from 'bootstrap';

export default {
  data() {
    return {
      users: []
    };
  },
  methods: {
    async getEnterprises() {
      try {
        const response = await axios.get(API_URL + '/Developer/getUsers');
        this.users = response.data;
        this.$nextTick(() => {
          const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
          popoverTriggerList.map(function (popoverTriggerEl) {
            return new Popover(popoverTriggerEl, {
              customClass: 'custom-popover'
            });
          });
        });
      } catch (error) {
        console.error('Error fetching enterprises:', error);
      }
    },
    goToMainPage() {
      window.location.href = "/";
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
  grid-template-columns: repeat(4, 1fr);
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

.popover.custom-popover {
  --bs-popover-bg: #f8f9fa;
  --bs-popover-color: #333;
  border-radius: 10px;
  max-width: 300px;
  box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
}

.custom-popover .popover-header {
  background-color: #02174B;
  color: #D0EDA0;
  font-size: 1.2rem;
  padding: 10px;
}

.custom-popover .popover-body {
  background-color: #D0EDA0;
  color: #02174B;
  font-size: 1rem;
  padding: 10px;
}

.custom-popover-btn {
  background-color: #49505700;
  color: #fff;
  border-radius: 20px;
  padding: 5px 15px;
  font-weight: bold;
}

.nav-tabs .nav-link.active {
  background-color: #8FA3BE;
  color: #91AC65;
}

.custom-popover-btn:hover {
  background-color: #343a4000;
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
