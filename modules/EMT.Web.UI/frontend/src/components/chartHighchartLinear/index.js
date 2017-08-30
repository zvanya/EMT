import { connect } from 'react-redux'
import 'whatwg-fetch'
import store from '../../store'

import 'highcharts/highmaps.js'
import ReactHighstock from 'react-highcharts/ReactHighstock.src'

const Highcharts = require('highcharts')
require('highcharts/modules/boost')(Highcharts)

const getConfig = props => ({
    chart: {
        type: props['chart-type']
    },
    rangeSelector: {
        // enabled: false
        buttons: [
            {
                count: 30,
                type: 'second',
                text: '30s'
            },
            {
                count: 1,
                type: 'minute',
                text: '1M'
            }, {
                type: 'all',
                text: 'All'
            }],
        inputEnabled: false,
        selected: 0
    },

    title: {
        text: props.title
    },

    exporting: {
        enabled: false
    }
})

class Chart extends React.Component {

    componentDidMount() {
        this.chart = this.refs.chart.getChart()
        this.updateChart()
    }

    shouldComponentUpdate() {
        return false
    }

    updateChart() {
        if (this.chart && this.props.data && this.props.data.length) {
            this.props.data.forEach(serie => {
                const existingSerie = this.chart.series && this.chart.series.filter(s => s.options.id === serie.id)[0]

                if (existingSerie) {
                    serie.data.forEach(point => {
                        existingSerie.addPoint(point, false, true)
                    })
                    this.chart.redraw()
                } else {
                    if (serie.data.length) {
                        this.chart.addSeries(serie)
                    }
                }
            })
        }
    }

    componentWillReceiveProps() {
        this.updateChart()
    }

    render() {
        return (
            <ReactHighstock
                config={getConfig(this.props)}
                ref="chart"
                isPureConfig
                ></ReactHighstock>
        )
    }
}

export default Chart
