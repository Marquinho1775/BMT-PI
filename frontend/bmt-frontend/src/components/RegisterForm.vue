<template>

  <body>
    <div class="d-flex justify-content-center align-items-center vh-100">
      <div id="form" class="card p-4 shadow" style="max-width: 400px; width: 100%">

        <h3 id="titulo" class="text-center align-items-cente p-3 rounded">
          Registro de usuario
        </h3>

        <b-form @submit.prevent="registerUser" @reset="onReset">

          <!-- Name -->
          <b-form-group id="input-group-name" label="Nombre:">
            <b-form-input id="name" v-model="datosFormulario.Name" placeholder="Ingresar nombre"
              required></b-form-input>
          </b-form-group>

          <!-- LastName -->
          <b-form-group id="input-group-lastname" label="Apellidos:">
            <b-form-input id="lastName" v-model="datosFormulario.LastName" placeholder="Ingresar apellidos"
              required></b-form-input>
          </b-form-group>

          <!-- Username -->
          <b-form-group id="input-group-username" label="Nombre de usuario:" label-for="username">
            <b-form-input id="username" v-model="datosFormulario.Username" placeholder="Ingresar nombre de usuario"
              required></b-form-input>
          </b-form-group>

          <!-- Email Address -->
          <b-form-group id="input-group-email" label="Correo electronico:" label-for="email">
            <b-form-input id="email" v-model="datosFormulario.Email" type="email"
              placeholder="Ingresar correo electronico" required></b-form-input>
          </b-form-group>

          <!-- Password -->
          <b-form-group id="input-group-password" label="Contraseña:" label-for="password">
            <b-form-input id="password" v-model="datosFormulario.Password" type="password"
              placeholder="Ingresar contraseña" required></b-form-input>
          </b-form-group>

          <!-- Submit and Reset Buttons -->
          <div class="d-flex justify-content-between">
            <b-button variant="secondary" @click="Volver">Volver</b-button>
            <b-button variant="secondary">Limpiar</b-button>
            <b-button class="boton">Registrar</b-button>
          </div>
        </b-form>
      </div>
    </div>
  </body>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      datosFormulario: {
        Name: '',
        LastName: '',
        Username: '',
        Email: '',
        isVerified: true,
        Password: ''
      }
    };
  },
  methods: {
    registerUser() {
      axios.post('https://localhost:7189/api/User', {
        Name: this.datosFormulario.Name,
        LastName: this.datosFormulario.LastName,
        Username: this.datosFormulario.Username,
        Email: this.datosFormulario.Email,
        isVerified: this.datosFormulario.isVerified,
        Password: this.datosFormulario.Password,

      }).then((response) => {

        this.$swal.fire({
          title: 'Registro exitoso',
          text: '¡El usuario ha sido registrado correctamente!',
          icon: 'success',
          confirmButtonText: 'Ok'
        }).then(() => {

          console.log(response);
          window.location.href = "/";
        });
      }).catch((error) => {
        // Error alert
        this.$swal.fire({
          title: 'Error',
          text: 'Hubo un error al registrar el usuario. Inténtalo de nuevo.',
          icon: 'error',
          confirmButtonText: 'Ok'
        });
        console.log(error);
      });
    },
    onReset(event) {
      event.preventDefault();
      // Reset form values
      this.datosFormulario.Name = '';
      this.datosFormulario.LastName = '';
      this.datosFormulario.Username = '';
      this.datosFormulario.Email = '';
      this.datosFormulario.Password = '';

    },
    Volver() {
      window.location.href = "/";
    }
  }
};
</script>

<style scoped>
body {
  background-color: #D1E4FF;
}

.boton {
  background-color: #39517B;
}

.boton:hover {
  background-color: #02174B;
}

#form {
  background-color: #9FC9FC;
}

#titulo {
  color: white;
  background-color: #39517B;
}

#name {
  background-color: #D0EDA0;
}

#username {
  background-color: #D0EDA0;
}

#lastName {
  background-color: #D0EDA0;
}

#email {
  background-color: #D0EDA0;
}

#password {
  background-color: #D0EDA0;
}
</style>
