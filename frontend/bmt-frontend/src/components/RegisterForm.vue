<template>
    <div class="d-flex justify-content-center align-items-center vh-100">
        <div class="card p-4 shadow" style="max-width: 400px; width: 100%">
            <h3 class="text-center">
                Formulario de creacion de usuario
            </h3>
            <form @submit.prevent="registerUser">
                <div>
                    <label for="name">Nombre</label>
                    <input type="text" id="name" name="name" required>
                </div>
                <div>
                    <label for="username">Nombre de usuario</label>
                    <input type="text" id="username" name="username" required>
                </div>
                <div>
                    <label for="email">Email</label>
                    <input type="email" id="email" name="email" required>
                </div>
                <div>
                    <label for="password">Contraseña</label>
                    <input type="password" id="password" name="password" required>
                </div>
                <div>
                    <label for="isEntrepeneur">¿Quiere ser emprendedor?</label>
                    <input type="checkbox" id="isEntrepeneur" name="isEntrepeneur">
                </div>
                <div>
                    <label for="identification">Cédula</label>
                    <input type="text" id="identification" name="identification" required>
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
                    IsVerified: false,
                    Password: '',
                    IsEntrepeneur: false,
                    Identification: ''
                },
            };
        },
        methods: {
            registerUser() {
                console.log("Datos a guardar:",this.datosFormulario);
                
                axios.post('https://localhost:7189/api/User', {
                    Name: this.Name,
                    Username: this.Username,
                    Email: this.Email,
                    IsVerified: this.IsVerified,
                    Password: this.Password,
                    IsEntrepeneur: this.IsEntrepeneur,
                    Identification: this.Identification
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