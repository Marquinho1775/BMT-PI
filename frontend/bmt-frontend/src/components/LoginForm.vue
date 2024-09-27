<template>

    <body>
        <div class="d-flex justify-content-center align-items-center vh-100">
            <div id="formLogin" class="card p-4 shadow" style="max-width: 400px; width: 100%">
                <h3 id="tituloLogin" class="text-center">Iniciar sesión</h3>
                <b-form @submit.prevent="loginUser">

                    <!-- Email -->
                    <b-form-group id="input-group-email" label="Correo electrónico:">
                        <b-form-input id="email" v-model="loginForm.Email" type="email"
                            placeholder="Ingresar correo electrónico" required></b-form-input>
                    </b-form-group>

                    <!-- Password -->
                    <b-form-group id="input-group-password" label="Contraseña:">
                        <b-form-input id="password" v-model="loginForm.Password" type="password"
                            placeholder="Ingresar contraseña" required></b-form-input>
                    </b-form-group>

                    <!-- Submit Button -->
                    <div class="d-flex justify-content-between">
                        <b-button variant="secondary" @click="Volver">Volver</b-button>
                        <b-button variant="primary" type="submit">Iniciar sesión</b-button>

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
            loginForm: {
                Email: '',
                Password: ''
            }
        };
    },
    methods: {
        async loginUser() {
            try {
                console.log({
                    Email: this.loginForm.Email,
                    Password: this.loginForm.Password
                });
                const response = await axios.post('https://localhost:7189/api/User/login',
                    {
                        Email: this.loginForm.Email.trim(),
                        Password: this.loginForm.Password.trim()
                    },
                    {
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    }
                );

                // Store token and user information in local storage
                localStorage.setItem('token', response.data.Token);
                localStorage.setItem('user', JSON.stringify(response.data.User));

                this.$swal.fire({
                    title: 'Registro exitoso',
                    text: '¡Haz iniciado sesión correctamente!',
                    icon: 'success',
                    confirmButtonText: 'Ok'
                }).then(() => {

                    window.location.href = "/";
                });
            } catch (error) {
                console.error(error);
                this.$swal.fire({
                    title: 'Error',
                    text: 'Hubo un error al iniciar sesión. Inténtalo de nuevo.',
                    icon: 'error',
                    confirmButtonText: 'Ok'
                });
            }
        },
        Volver() {
            window.location.href = "/";
        }
    },
};
</script>

<style></style>