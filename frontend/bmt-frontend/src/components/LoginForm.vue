<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card p-4 shadow" style="max-width: 400px; width: 100%">
      <h3 class="text-center">
        Iniciar sesión
      </h3>
      <b-form @submit.prevent="loginUser">
        <!-- Email -->
        <b-form-group id="input-group-email" label="Correo electrónico:">
          <b-form-input id="email" v-model="loginForm.Email" type="email" placeholder="Ingresar correo electrónico"
            required></b-form-input>
        </b-form-group>

        <!-- Password -->
        <b-form-group id="input-group-password" label="Contraseña:">
          <b-form-input id="password" v-model="loginForm.Password" type="password" placeholder="Ingresar contraseña"
            required></b-form-input>
        </b-form-group>

        <!-- Submit Button -->
        <div class="d-flex justify-content-between">
          <b-button type="submit" variant="primary">Iniciar sesión</b-button>
        </div>
      </b-form>
    </div>
  </div>
</template>

<script>
import { login } from '../services/authService'; // Asegúrate de que la ruta esté correcta

export default {
  data() {
    return {
      loginForm: {
        Email: '',
        Password: ''
      }
    };
  },
  methods: {
    async loginUser() {
      try {
        const response = await login(this.loginForm.Email, this.loginForm.Password);

        // Guardar el token y redirigir si el login fue exitoso
        this.$swal.fire({
          title: 'Inicio de sesión exitoso',
          text: '¡Has iniciado sesión correctamente!',
          icon: 'success',
          confirmButtonText: 'Ok'
        }).then(() => {
          console.log(response);
          window.location.href = "/"; // Redirige a la página de inicio
        });
      } catch (error) {
        // Mostrar mensaje de error
        this.$swal.fire({
          title: 'Error',
          text: 'Credenciales incorrectas. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        console.error(error);
      }
    }
  }
};
</script>

<style></style>