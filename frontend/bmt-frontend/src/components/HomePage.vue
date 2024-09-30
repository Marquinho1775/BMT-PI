<template>
  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de users</h1>
    <div class="row justify-content-end">
      <div class="col-2">
        <!-- Mostrar los botones de registro e inicio de sesión solo si no está autenticado -->
        <div v-if="!isLoggedIn">
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
        </div>

        <!-- Mostrar el botón de logout si está autenticado -->
        <div v-else>
          <button @click="logout" type="button" class="btn btn-outline-danger float-right">
            Logout
          </button>
        </div>
          <router-link to="/enterprise-register">
              <button type="button" class="btn btn-outline-secondary float-right">
                Register enterprise
             </button>
        </router-link>
        <router-link to="/enterprises">
            <button type="button" class="btn btn-outline-primary float-right mr-2">
              Mis emprendimientos
            </button>
          </router-link>
      </div>
    </div>
  </div>

  <div v-if="isLoggedIn" class="container mt-3 text-center">
    <p class="alert alert-success">¡Bienvenido, has iniciado sesión!</p>
  </div>

  <div class="container mt-5">
    <h1 class="display-4 text-center">Lista de users</h1>
    <table class="table is-bordered is-striped is-narrow is-hoverable is-fullwidth">
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
      isLoggedIn: false // Nuevo estado para verificar si el usuario ha iniciado sesión
    };
  },
  methods: {
    obtenerTareas() {
      axios.get("https://localhost:7189/api/User").then(
        (response) => {
          this.users = response.data;
        });
    },
    // Método para cerrar sesión
    logout() {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.isLoggedIn = false;
      this.$swal.fire({
        title: 'Sesión cerrada',
        text: 'Has cerrado sesión exitosamente.',
        icon: 'success',
        confirmButtonText: 'Ok'
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
  created() {
    this.obtenerTareas();

    // Comprobar si el token existe en el localStorage para saber si el usuario ha iniciado sesión
    const token = localStorage.getItem('token');
    this.isLoggedIn = !!token; // Establece isLoggedIn como true si el token existe
    this.getEnteprises();
  },
};
</script>

<style lang="scss" scoped></style>
