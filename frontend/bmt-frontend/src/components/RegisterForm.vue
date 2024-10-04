<template>
  <div class="d-flex justify-content-center align-items-center vh-100">
    <div id="form" class="card custom-card" style="max-width: 400px; width: 100%">
      <h3 id="titulo" class="text-center card-header-custom">
        Registro de usuario
      </h3>
      <div class="card-body">
        <b-form @submit.prevent="registerUser" @reset="onReset">
          <b-form-group id="input-group-name" label="Nombre: *">
            <b-form-input class="input_place" v-model="formData.Name" placeholder="Ingresar nombre" required
              maxlength="20" :state="nameValid" @input="validateName"></b-form-input>
            <b-form-invalid-feedback v-if="!nameValid">
              El nombre solo puede contener letras y tener máximo 20 caracteres.
            </b-form-invalid-feedback>
          </b-form-group>

          <b-form-group id="input-group-lastname" label="Apellidos: *">
            <b-form-input class="input_place" v-model="formData.LastName" placeholder="Ingresar apellidos" required
              maxlength="50" :state="lastnameValid" @input="validateLastName"></b-form-input>
            <b-form-invalid-feedback v-if="!lastnameValid">
              Los apellidos solo pueden contener letras y tener máximo 50 caracteres.
            </b-form-invalid-feedback>
          </b-form-group>

          <b-form-group id="input-group-username" label="Nombre de usuario: *" label-for="username">
            <b-form-input class="input_place" v-model="formData.Username" placeholder="Ingresar nombre de usuario"
              required maxlength="20" :state="usernameValid" @input="validateUsername"></b-form-input>
            <b-form-invalid-feedback v-if="!usernameValid">
              El nombre de usuario solo puede contener letras, números, y los caracteres especiales -_´.
            </b-form-invalid-feedback>
          </b-form-group>

          <b-form-group id="input-group-email" label="Correo electronico: *" label-for="email">
            <b-form-input class="input_place" v-model="formData.Email" type="email"
              placeholder="Ingresar correo electronico" required :state="emailValid"
              @input="validateEmail"></b-form-input>
            <b-form-invalid-feedback v-if="!emailValid">
              El correo debe tener el formato xxxx@xxx.xxx
            </b-form-invalid-feedback>
          </b-form-group>

          <b-form-group id="input-group-password" label="Contraseña: *" label-for="password">
            <b-form-input class="input_place" v-model="formData.Password" type="password"
              placeholder="Ingresar contraseña" required maxlength="16" :state="passwordValid"
              @input="validatePassword"></b-form-input>
            <b-form-invalid-feedback v-if="!passwordValid">
              La contraseña debe tener máximo 16 caracteres, incluir una mayúscula, una minúscula, un número y un
              carácter especial.
            </b-form-invalid-feedback>
          </b-form-group>

          <b-form-group id="input-group-confirm-password" label="Confirmar Contraseña: *" label-for="confirm-password">
            <b-form-input class="input_place" v-model="confirmPassword" type="password"
              placeholder="Confirmar contraseña" required></b-form-input>
            <b-form-invalid-feedback v-if="!passwordMismatch">
              No coinciden las contraseñas.
            </b-form-invalid-feedback>
            <p v-if="passwordMismatch" class="text-danger">No coinciden las contraseñas</p>
          </b-form-group>

          <div class="d-flex justify-content-between">
            <b-button variant="secondary" @click="Volver">Volver</b-button>
            <b-button variant="secondary">Limpiar</b-button>
            <b-button type="submit" class="button" :disabled="passwordMismatch">Registrar</b-button>
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
      confirmPassword: '',
    };
  },
  computed: {
    passwordMismatch() {
      return this.formData.Password !== this.confirmPassword;
    },
  },
  methods: {
    registerUser() {
      if (!this.passwordMismatch) {
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
      }
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
      const regex = /^[a-zA-Z0-9-_´.]{1,20}$/;
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
