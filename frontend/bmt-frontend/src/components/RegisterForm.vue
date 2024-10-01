<template>

  <div class="d-flex justify-content-center align-items-center vh-100">
    <div id="form" class="card custom-card" style="max-width: 400px; width: 100%">
      <h3 id="titulo" class="text-center card-header-custom">
        Registro de usuario
      </h3>
      <div class="card-body">
        <b-form @submit.prevent="registerUser" @reset="onReset">
          <b-form-group id="input-group-name" label="Nombre:">
            <b-form-input class="input_place" v-model="formData.Name" placeholder="Ingresar nombre"
              required></b-form-input>
          </b-form-group>

          <b-form-group id="input-group-lastname" label="Apellidos:">
            <b-form-input class="input_place" v-model="formData.LastName" placeholder="Ingresar apellidos"
              required></b-form-input>
          </b-form-group>

          <b-form-group id="input-group-username" label="Nombre de usuario:" label-for="username">
            <b-form-input class="input_place" v-model="formData.Username" placeholder="Ingresar nombre de usuario"
              required></b-form-input>
          </b-form-group>

          <b-form-group id="input-group-email" label="Correo electronico:" label-for="email">
            <b-form-input class="input_place" v-model="formData.Email" type="email"
              placeholder="Ingresar correo electronico" required></b-form-input>
          </b-form-group>

          <b-form-group id="input-group-password" label="Contraseña:" label-for="password">
            <b-form-input class="input_place" v-model="formData.Password" type="password"
              placeholder="Ingresar contraseña" required></b-form-input>
          </b-form-group>

          <div class="d-flex justify-content-between">
            <b-button variant="secondary" @click="Volver">Volver</b-button>
            <b-button variant="secondary">Limpiar</b-button>
            <b-button type="submit" class="button">Registrar</b-button>
          </div>
        </b-form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      formData: {
        Id: '',
        Name: '',
        LastName: '',
        Username: '',
        Email: '',
        isVerified: false,
        Password: '',
      },
    };
  },
  methods: {
    registerUser() {
      axios.post('https://localhost:7189/api/User', {
        Id: this.formData.Id,
        Name: this.formData.Name,
        LastName: this.formData.LastName,
        Username: this.formData.Username,
        Email: this.formData.Email,
        isVerified: this.formData.isVerified,
        Password: this.formData.Password,
      })
        .then((response) => {
          this.$swal.fire({
            title: 'Registro exitoso',
            text: '¡El usuario ha sido registrado correctamente!',
            icon: 'success',
            confirmButtonText: 'Ok'
          }).then(() => {
            console.log(response);
            window.history.back();
          });
        })
        .catch((error) => {
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
      this.formData.Name = '';
      this.formData.LastName = '';
      this.formData.Username = '';
      this.formData.Email = '';
      this.formData.Password = '';
    },
    Volver() {
      window.location.href = "/";
    },
  },
  created() {

  }
};
</script>


<style scoped>
body {
  background-color: #D1E4FF;
}

.custom-card {
  width: 650px;
  background-color: #9FC9FC;
  border-radius: 20px;
  margin: 0px;
}

.card-header-custom {
  background-color: #36618E;
  color: white;
  padding: 20px;
  border-radius: 20px 20px 0 0;
  width: 100%;
  height: 100%;
}

.button {
  background-color: #39517B;
}

.button:hover {
  background-color: #02174B;
}

.input_place {
  background-color: #D0EDA0;
}

#form {
  background-color: #9FC9FC;
}

#titulo {
  color: white;
  background-color: #39517B;
}
</style>
