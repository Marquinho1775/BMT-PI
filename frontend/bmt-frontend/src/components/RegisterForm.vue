<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div class="card p-4 shadow" style="max-width: 400px; width: 100%">

      <h3 class="text-center">
        Registro de usuario
      </h3>

      <b-form @submit.prevent="registerUser" @reset="onReset">

        <!-- Name -->
        <b-form-group id="input-group-name" label="Nombre:">
          <b-form-input id="name" v-model="datosFormulario.Name" placeholder="Ingresar nombre" required></b-form-input>
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

        <!-- Entrepreneur Checkbox -->
        <b-form-group id="input-group-isentrepeneur" label-for="Quieres ser emprendedor?">
          <b-form-checkbox id="isEntrepeneur" v-model="datosFormulario.IsEntrepeneur" name="isEntrepeneur">
            Quieres ser emprendedor?
          </b-form-checkbox>
        </b-form-group>

        <!-- Identification -->
        <b-form-group id="input-group-identification" label="Identificación:" label-for="identification">
          <b-form-input id="input-identification" v-model="datosFormulario.Identification"
            placeholder="Ingresar identificación" required></b-form-input>
        </b-form-group>

        <!-- Submit and Reset Buttons -->
        <div class="d-flex justify-content-between">
          <b-button variant="info" @click="Volver">Volver</b-button>
          <b-button type="reset" variant="secondary">Reiniciar</b-button>
          <b-button type="submit" variant="primary">Registrar</b-button>
        </div>
      </b-form>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      datosFormulario: {
        Name: '',
        Username: '',
        Email: '',
        isVerified: true,
        Password: '',
        IsEntrepeneur: false,
        Identification: ''
      }
    };
  },
  methods: {
    registerUser() {
      axios.post('https://localhost:7189/api/User', {
        Id: "",
        Name: this.datosFormulario.Name,
        Username: this.datosFormulario.Username,
        Email: this.datosFormulario.Email,
        isVerified: this.datosFormulario.isVerified,
        Password: this.datosFormulario.Password,
        IsEntrepeneur: this.datosFormulario.IsEntrepeneur,
        Identification: this.datosFormulario.Identification

      }).then((response) => {
        // Success alert
        console.log(this.registerUser.ProfilePicture);

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
      this.datosFormulario.Username = '';
      this.datosFormulario.Email = '';
      this.datosFormulario.Password = '';
      this.datosFormulario.IsEntrepeneur = false;
      this.datosFormulario.Identification = '';
    },
    Volver() {
      window.location.href = "/";
    }
  }
};
</script>

<style>
.bg-color {
  background-color: #D1E4FF;
  /* Aquí puedes cambiar el color al que desees */
}
</style>