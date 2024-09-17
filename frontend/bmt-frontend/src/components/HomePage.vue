<template>
    <div class="container mt-5">
        <h1 class="display-4 text-center"> Lista de users </h1>
        <div class="row justify-content-end">
            <div class="col-2">
                <router-link to="/register">
                    <button type="button" class="btn btn-outline-secondary float-right">
                        Agregar user
                    </button>
                </router-link>
            </div>
        </div>
    </div>

    <div class="container mt-5">
        <h1 class="display-4 text-center">Lista de users</h1>
        <table
        class="table is-bordered is-striped is-narrow is-hoverable
        is-fullwidth"
        >
            <thead>
                <tr>
                    <th>Nombre</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(user, index) of users" :key="index">
                    <td>{{ user.name }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
    import axios from "axios";
    export default {
        name: 'HomePage',
        data() {
            return {
                users: [],
            };
        },
        methods: {
            obtenerTareas() {
                axios.get("https://localhost:7189/api/User").then(
                (response) => {
                    this.users = response.data;
                });
            },
            
        },
        created: function () {
                this.obtenerTareas();
            },
    };
    
</script>

<style lang="scss" scoped>

</style>