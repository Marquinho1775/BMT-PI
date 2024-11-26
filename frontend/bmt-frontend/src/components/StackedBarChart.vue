<template>
  <v-card>
    <v-card-title class="text-h6">
      {{ type === 0 ? 'Ganancias Mensuales' : 'Ganancias Semanales' }}
    </v-card-title>
    <v-card-text>
      <div class="chart-container">
        <Bar
          id="my-chart-id"
          :data="chartData"
          :options="chartOptions"
        />
      </div>
    </v-card-text>
  </v-card>
</template>

<script>
import { Bar } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from 'chart.js';

ChartJS.register(
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
  {
    id: 'totalLabelPlugin',
    afterDatasetsDraw(chart) {
      const { ctx, scales: { x, y } } = chart;
      const datasets = chart.data.datasets;
      const labels = chart.data.labels;

      labels.forEach((label, index) => {
        const total = datasets.reduce((sum, dataset) => sum + (dataset.data[index] || 0), 0);
        const xPos = x.getPixelForValue(index);
        const yPos = y.getPixelForValue(total);

        ctx.save();
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        ctx.fillStyle = 'black';
        ctx.font = 'bold 12px Arial';
        ctx.fillText(total, xPos, yPos - 5);
        ctx.restore();
      });
    },
  }
);

export default {
  name: 'StackedBarChart',
  components: { Bar },
  props: {
    datasets: {
      type: Array,
      required: true,
    },
    labels: {
      type: Array,
      required: false,
      default: () => [
        'Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun',
        'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'
      ],
    },
    type: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        scales: {
          x: {
            stacked: true,
            title: {
              display: true,
              text: this.type === 0 ? 'Mes' : 'Día de la Semana',
            },
            ticks: {
              maxRotation: 0,
              minRotation: 0,
            },
          },
          y: {
            stacked: true,
            title: {
              display: true,
              text: 'Ganancias (colones)',
            },
            beginAtZero: true,
          },
        },
        plugins: {
          legend: {
            position: 'top',
            labels: {
              boxWidth: 20,
              font: {
                size: 12,
              },
            },
          },
          tooltip: {
            mode: 'index',
            intersect: false,
            callbacks: {
              label: function (context) {
                const dataset = context.dataset;
                const dataIndex = context.dataIndex;
                const datasets = context.chart.data.datasets;

                const total = datasets.reduce((sum, dataset) => {
                  const value = dataset.data[dataIndex];
                  return sum + (isNaN(value) ? 0 : value);
                }, 0);

                const currentValue = dataset.data[dataIndex];
                const percentage = ((currentValue / total) * 100).toFixed(1);

                let label = dataset.label || '';
                if (label) {
                  label += ': ';
                }
                label += `${currentValue} (${percentage}%)`;
                return label;
              },
            },
          },
        },
      },
    };
  },
  computed: {
    chartData() {
      return {
        labels: this.labels,
        datasets: this.datasets,
      };
    },
  },
};
</script>

<style scoped>
.v-card {
  margin-top: 20px;
}

.chart-container {
  width: 100%;
  overflow-x: auto;
  height: 700px;
}

.chart-container ::v-deep canvas {
  min-width: 600px;
}

@media (max-width: 600px) {
  .chart-container {
    height: 300px;
  }
  .chart-container ::v-deep canvas {
    min-width: 500px;
  }
  .v-card-title {
    font-size: 16px;
  }
}
</style>
