<template>
  <v-main class="d-flex justify-center align-center" style="min-height: 100vh;">
    <v-container max-width="500px">
      <v-card class="pa-4 elevation-2">
        <v-card-title class="text-h5 text-center">Registro de usuario</v-card-title>
        <v-divider></v-divider>

        <v-card-text>
          <v-form @submit.prevent="registerUser" @reset="onReset">
            <!-- Nombre -->
            <v-text-field
              label="Nombre"
              v-model="formData.Name"
              placeholder="Ingresar nombre"
              maxlength="20"
              :error-messages="!nameValid ? ['El nombre solo puede contener letras y tener máximo 20 caracteres.'] : []"
              @input="validateName"
              required
              outlined
            ></v-text-field>

            <!-- Apellidos -->
            <v-text-field
              label="Apellidos"
              v-model="formData.LastName"
              placeholder="Ingresar apellidos"
              maxlength="50"
              :error-messages="!lastnameValid ? ['Los apellidos solo pueden contener letras y tener máximo 50 caracteres.'] : []"
              @input="validateLastName"
              required
              outlined
            ></v-text-field>

            <!-- Nombre de usuario -->
            <v-text-field
              label="Nombre de usuario"
              v-model="formData.Username"
              placeholder="Ingresar nombre de usuario"
              maxlength="20"
              :error-messages="!usernameValid ? ['El nombre de usuario solo puede contener letras, números, y los caracteres especiales -_´.'] : []"
              @input="validateUsername"
              required
              outlined
            ></v-text-field>

            <!-- Correo electrónico -->
            <v-text-field
              label="Correo electrónico"
              v-model="formData.Email"
              type="email"
              placeholder="Ingresar correo electrónico"
              :error-messages="!emailValid ? ['El correo debe tener el formato xxxx@xxx.xxx'] : []"
              @input="validateEmail"
              required
              outlined
            ></v-text-field>

            <!-- Contraseña -->
            <v-text-field
              label="Contraseña"
              v-model="formData.Password"
              type="password"
              placeholder="Ingresar contraseña"
              maxlength="16"
              :error-messages="!passwordValid ? ['La contraseña debe tener máximo 16 caracteres, incluir una mayúscula, una minúscula, un número y un carácter especial.'] : []"
              @input="validatePassword"
              required
              outlined
            ></v-text-field>

            <!-- Confirmar contraseña -->
            <v-text-field
              label="Confirmar contraseña"
              v-model="confirmPassword"
              type="password"
              placeholder="Confirmar contraseña"
              :error-messages="passwordMismatch ? ['No coinciden las contraseñas.'] : []"
              required
              outlined
            ></v-text-field>

            <!-- Botones de acción -->
            <div class="d-flex justify-end mt-4">
              <v-btn color="secondary" class="mr-2" outlined @click="Volver">Volver</v-btn>
              <v-btn type="reset" color="secondary" outlined>Limpiar</v-btn>
              <v-btn type="submit" color="primary" :disabled="passwordMismatch">Registrar</v-btn>
            </div>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
  </v-main>
</template>


<script>
import axios from 'axios';
import { API_URL } from '@/main.js';

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
      confirmPassword: '',
    };
  },
  computed: {
    passwordMismatch() {
      return this.formData.Password !== this.confirmPassword;
    },
  },
  methods: {
    async registerUser() {
      if (!this.passwordMismatch) {
        await axios.post(API_URL + '/User', {
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
      }
      await axios.post(API_URL + '/ShoppingCart?userName=' + this.formData.Username, null)
        .then((response) => {
          console.log("Shopping cart created: ", response);
        })
        .catch((error) => {
          console.error("Error creating shopping cart: ", error);
        });
    },
    onReset(event) {
      event.preventDefault();
      this.formData.Name = '';
      this.formData.LastName = '';
      this.formData.Username = '';
      this.formData.Email = '';
      this.formData.Password = '';
      this.confirmPassword = '';
    },
    Volver() {
      window.location.href = "/";
    },
    validateName() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,20}$/;
      this.nameValid = regex.test(this.formData.Name);
    },
    validateLastName() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,50}$/;
      this.lastnameValid = regex.test(this.formData.LastName);
    },
    validateUsername() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9-_´.]{1,20}$/;
      this.usernameValid = regex.test(this.formData.Username);
    },
    validateEmail() {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      this.emailValid = regex.test(this.formData.Email);
    },
    validatePassword() {
      const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{1,16}$/;
      this.passwordValid = regex.test(this.formData.Password);
    },
  },
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