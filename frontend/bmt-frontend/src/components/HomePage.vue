<template>
  <div class="container mt-5">
    <div class="container mt-5 row">
      <div class="row-2">
        <router-link to="/register">
          <button type="button" class="btn btn-outline-secondary float-right">
            Register user
          </button>
        </router-link>
        <router-link to="/login">
          <button type="button" class="btn btn-outline-secondary float-right">
            Login user
          </button>
        </router-link>
        <router-link to="/enterprise-register">
          <button type="button" class="btn btn-outline-secondary float-right">
            Register enterprise
          </button>
        </router-link>
      </div>
    </div>
  </div>

  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de users</h1>
    <table class="table is-bordered is-striped is-narrow is-hoverable
      is-fullwidth">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Correo</th>
          <th>Contraseña</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(user, index) of users" :key="index">
          <td>{{ user.name }}</td>
          <td>{{ user.email }}</td>
          <td>{{ user.password }}</td>
        </tr>
      </tbody>
    </table>
  </div>

  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de emprendimientos</h1>
    <table class="table is-bordered is-striped is-narrow is-hoverable
      is-fullwidth">
      <thead>
        <tr>
          <th>Nombre</th>
          <th>Descripción</th>
          <th>Administrador</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(enterprise, index) of enterprises" :key="index">
          <td>{{ enterprise.name }}</td>
          <td>{{ enterprise.description }}</td>
          <td>{{ enterprise.administrator ? enterprise.administrator.name : 'Sin administrador' }}</td>
        </tr>
      </tbody>

    </table>
  </div>

</template>

<script>
import axios from "axios";
export default {
  name: 'HomePage',
  data() {
    return {
      users: [],
      enterprises: [],
    };
  },
  methods: {
    obtenerTareas() {
      axios.get("https://localhost:7189/api/User").then(
        (response) => {
          this.users = response.data;
        });
    },
    getEnteprises() {
      axios.get("https://localhost:7189/api/Enterprise").then(
        (response) => {
          this.enterprises = response.data;
          console.log(this.enterprises);
        });
    },
    

  },
  created: function () {
    this.obtenerTareas();
    this.getEnteprises();
  },
};

</script>

<style lang="scss" scoped></style>