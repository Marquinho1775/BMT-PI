<template>
    <div class="d-flex justify-content-center align-items-center vh-100">
        <div class="card p-4 shadow" style="max-width: 400px; width: 100%">
            <h3 class="text-center">
                Formulario de creacion de usuario
            </h3>
            <form @submit.prevent="registerUser">
                <div>
                    <label for="name">Nombre</label>
                    <input v-model="datosFormulario.Name"
                     type="text" id="name" name="name" required>
                </div>
                <div>
                    <label for="username">Nombre de usuario</label>
                    <input v-model="datosFormulario.Username"
                    type="text" id="username" name="username" required>
                </div>
                <div>
                    <label for="email">Email</label>
                    <input v-model="datosFormulario.Email"
                    type="email" id="email" name="email" required>
                </div>
                <div>
                    <label for="password">Contraseña</label>
                    <input v-model="datosFormulario.Password"
                    type="password" id="password" name="password" required>
                </div>
                <div>
                    <label for="isEntrepeneur">¿Quiere ser emprendedor?</label>
                    <input v-model="datosFormulario.IsEntrepeneur"
                    type="checkbox" id="isEntrepeneur" name="isEntrepeneur">
                </div>
                <div>
                    <label for="identification">Cédula</label>
                    <input v-model="datosFormulario.Identification"
                    type="text" id="identification" name="identification" required>
                </div>
                <div>
                    <button type="submit" class="btn btn-success btn-block">Registrar</button>
                </div>
            </form>
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
                    IsVerified: true,
                    Password: '',
                    IsEntrepeneur: '',
                    Identification: ''
                },
            };
        },
        methods: {
            registerUser() {
                console.log("Datos a guardar:",this.datosFormulario);
                
                axios.post('https://localhost:7189/api/User', {
                    Id: "",
                    Name: this.datosFormulario.Name,
                    Username: this.datosFormulario.Username,
                    Email: this.datosFormulario.Email,
                    IsVerified: this.datosFormulario.IsVerified,
                    Password: this.datosFormulario.Password,
                    IsEntrepeneur: this.datosFormulario.IsEntrepeneur,
                    Identification: this.datosFormulario.Identification
                }).then((response) => {
                    console.log(response);
                    window.location.href = "/";
                })
                .catch((error) => {
                    console.log(error);
                });
            },
        },
    };
</script>

<style>

</style>