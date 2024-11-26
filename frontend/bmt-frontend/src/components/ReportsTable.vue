<template>
    <v-table class="reports-table">
        <thead>
            <tr>
                <th v-for="(title, index) in titles" :key="index" class="header1">
                    <strong>{{ title }}</strong>
                </th>
            </tr>
            <tr>
                <th v-for="(title, index) in titles" :key="'filter-' + index" class="filter-header">
                    <strong>
                        <input v-model="filters[index]" :placeholder="'Filtrar'" class="filter-input" />
                    </strong>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="(report, rowIndex) in filteredReports" :key="rowIndex">
                <td v-for="(key, cellIndex) in keys" :key="cellIndex">
                    {{ formatCell(report[key], key) }}
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
        keys: {
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
    data() {
        return {
            filters: [],
        };
    },
    created() {
        this.filters = this.titles.map(() => '');
    },
    watch: {
        titles(newTitles) {
            this.filters = newTitles.map(() => '');
        },
        keys() {
            this.filters = this.titles.map(() => '');
        },
    },
    computed: {
        filteredReports() {
            return this.reports.filter(report => {
                return this.keys.every((key, index) => {
                    const filter = this.filters[index].toLowerCase();
                    if (!filter) return true;
                    const cell = String(report[key] || '').toLowerCase();
                    return cell.includes(filter);
                });
            });
        },
    },
    methods: {
        formatCell(value, key) {
            if (key.includes('date')) {
                return new Date(value).toLocaleDateString();
            }
            if (typeof value === 'number') {
                return value.toLocaleString();
            }
            return value;
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

.header1 {
    background-color: #6991d1;
    /* Fondo celeste */
    text-align: left;
}

.reports-table tr:nth-child(even) {
    background-color: #f9f9f9;
}

.reports-table tr:hover {
    background-color: #ddd;
}

.filter-header {
    background-color: #e9e9e9ee;
    border: 1px solid white;
}

.filter-input {
    width: 100%;
    padding: 4px;
    box-sizing: border-box;
}
</style>
