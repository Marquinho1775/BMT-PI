<template>
    <v-table class="reports-table">
        <thead>
            <tr>
                <th v-for="(title, index) in titles" :key="index">
                    {{ title }}
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(report, rowIndex) in reports" :key="rowIndex">
                <td v-for="(cell, cellIndex) in report" :key="cellIndex">
                    {{ cell }}
                </td>
            </tr>
        </tbody>
    </v-table>
</template>

<script>
export default {
    name: 'ReportsTable',
    props: {
        titles: {
            type: Array,
            required: true,
            validator(value) {
                return value.length > 0;
            },
        },
        reports: {
            type: Array,
            required: true,
            validator(value) {
                return value.length > 0;
            },
        },
    },
};
</script>

<style scoped>
.reports-table {
    width: 100%;
    border-collapse: collapse;
}

.reports-table th,
.reports-table td {
    border: 1px solid #ddd;
    padding: 8px;
}

.reports-table th {
    background-color: #f2f2f2;
    text-align: left;
}

.reports-table tr:nth-child(even) {
    background-color: #f9f9f9;
}

.reports-table tr:hover {
    background-color: #ddd;
}
</style>