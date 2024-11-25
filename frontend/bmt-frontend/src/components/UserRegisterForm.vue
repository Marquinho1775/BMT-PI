<template>
  <v-main class="d-flex justify-center align-center" style="min-height: 100vh;">
    <v-container max-width="500px">
      <v-card class="pa-0 elevation-4">
        <v-card-text class="title-background">
          <v-card-title class="text-h5 text-center">Registro de usuario</v-card-title>
        </v-card-text>
        <v-divider></v-divider>

        <v-card-text>
          <v-form @submit.prevent="registerUser" @reset="onReset">
            <v-text-field label="Nombre" v-model="formData.Name" placeholder="Ingresar nombre" maxlength="20"
              :error-messages="touchedFields.Name && !validateName() ? ['El nombre solo puede contener letras y tener máximo 20 caracteres.'] : []"
              @blur="touchedFields.Name = true" outlined></v-text-field>

            <v-text-field label="Apellidos" v-model="formData.LastName" placeholder="Ingresar apellidos" maxlength="50"
              :error-messages="touchedFields.LastName && !validateLastName() ? ['Los apellidos solo pueden contener letras y tener máximo 50 caracteres.'] : []"
              @blur="touchedFields.LastName = true" outlined></v-text-field>

            <v-text-field label="Nombre de usuario" v-model="formData.Username" placeholder="Ingresar nombre de usuario"
              maxlength="20"
              :error-messages="touchedFields.Username && !validateUsername() ? ['El nombre de usuario solo puede contener letras, números, y los caracteres especiales -_´.'] : []"
              @blur="touchedFields.Username = true" outlined></v-text-field>

            <v-text-field label="Correo electrónico" v-model="formData.Email" type="email"
              placeholder="Ingresar correo electrónico"
              :error-messages="touchedFields.Email && !validateEmail() ? ['El correo debe tener el formato xxxx@xxx.xxx'] : []"
              @blur="touchedFields.Email = true" outlined></v-text-field>

            <v-text-field label="Contraseña" v-model="formData.Password" type="password"
              placeholder="Ingresar contraseña" maxlength="16"
              :error-messages="touchedFields.Password && !validatePassword() ? ['La contraseña debe tener máximo 16 caracteres, incluir una mayúscula, una minúscula, un número y un carácter especial.'] : []"
              @blur="touchedFields.Password = true" outlined></v-text-field>

            <v-text-field label="Confirmar contraseña" v-model="confirmPassword" type="password"
              placeholder="Confirmar contraseña"
              :error-messages="passwordMismatch ? ['No coinciden las contraseñas.'] : []"
              @blur="touchedFields.confirmPassword = true" outlined></v-text-field>

            <v-card-actions>
              <v-btn color="secondary" class="mr-2" outlined @click="Volver">Volver</v-btn>
              <v-btn type="reset" color="secondary" outlined>Limpiar</v-btn>
              <v-spacer />
              <v-btn type="submit" color="primary" :disabled="passwordMismatch">Registrar</v-btn>
            </v-card-actions>
          </v-form>
        </v-card-text>
      </v-card>
    </v-container>
  </v-main>
</template>


<script>
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
        nameValid: false,
        lastNameValid: false,
        usernameValid: false,
        emailValid: false,
        passwordValid: false,
      },
      confirmPassword: '',
      touchedFields: {
        Name: false,
        LastName: false,
        Username: false,
        Email: false,
        Password: false,
        confirmPassword: false,
      },
    };
  },
  computed: {
    passwordMismatch() {
      return (
        this.touchedFields.confirmPassword &&
        this.formData.Password !== this.confirmPassword
      );
    },
  },
  methods: {
    async registerUser() {
      if (!this.passwordMismatch) {
        // Registro de usuario
      }
    },
    onReset(event) {
      event.preventDefault();
      this.formData = {
        Id: '',
        Name: '',
        LastName: '',
        Username: '',
        Email: '',
        isVerified: false,
        Password: '',
      };
      this.confirmPassword = '';
      this.touchedFields = {
        Name: false,
        LastName: false,
        Username: false,
        Email: false,
        Password: false,
        confirmPassword: false,
      };
    },
    validateName() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,20}$/;
      return regex.test(this.formData.Name);
    },
    validateLastName() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ ]{1,50}$/;
      return regex.test(this.formData.LastName);
    },
    validateUsername() {
      const regex = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ0-9-_´.]{1,20}$/;
      return regex.test(this.formData.Username);
    },
    validateEmail() {
      const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return regex.test(this.formData.Email);
    },
    validatePassword() {
      const regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{1,16}$/;
      return regex.test(this.formData.Password);
    },
  },
};
</script>


<style scoped>
.title-background {
  background-color: #39517B;
  /* Fondo del encabezado */
  color: white;
  /* Texto blanco */
  padding: 16px;
  /* Espaciado interno */
  margin: 0;
  /* Elimina márgenes */
  border-radius: 4px 4px 0 0;
  /* Redondea las esquinas superiores */
  width: 100%;
  /* Abarca todo el ancho */
  display: block;
  /* Comportamiento de bloque */
  text-align: center;
  /* Centra el texto */
  box-sizing: border-box;
  /* Incluye el padding dentro del ancho total */
}
</style>