import { connect } from 'react-redux'

import ChartLinear from '../../components/chartHighchartLinear/'
import ChartCanvas from '../../components/chartCanvasLinear/'
import ChartD3 from '../../components/chartD3/'

const getConfig = props => (
    {
        rangeSelector: {
            buttons: [{
                count: 1,
                type: 'minute',
                text: '1M'
            }, {
                count: 5,
                type: 'second',
                text: 'Sec'
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
    }
)

class SeparateChartsPage extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            chartComponent: ChartLinear,
            chartType: 'line',
            compareMode: false
        }
    }

    switchComponentType(type) {
        console.log('switchComponentType: ', type)
        let chartComponent
        switch (type) {
            case 'Highcharts':
                this.setState({ chartComponent: ChartLinear })
                break
            case 'Canvas':
                this.setState({ chartComponent: ChartCanvas })
                break
            case 'D3':
                this.setState({ chartComponent: ChartD3 })
                break
            default:
                console.error(`Not supported component type '${type}'`)
        }
    }

    switchChartType(type) {
        console.log('switchChartType: ', type)
        switch (type) {
            case 'line':
                this.setState({ chartType: 'line' })
                break
            case 'column':
                this.setState({ chartType: 'column' })
                break
            default:
                console.error(`Not supported chart type '${type}'`)
        }
    }

    toggleCompareMode() {
        console.log('toggleCompareMode: ', !this.state.compareMode)
        this.setState({
            'compareMode': !this.state.compareMode
        })
    }

    renderSeparateCharts(Chart) {
        return this.props.selected_counters.map((scId, i) => {
            const formattedData = [{
                'id': scId,
                'name': this.props.counters.filter(c => c.id === scId)[0].name,
                'data': this.props.data[scId] || []
            }]
            if (this.props.data[scId]) {
                return (
                    <Chart
                        key={i}
                        chart-type={this.state.chartType}
                        title={formattedData[0].name}
                        data={formattedData}
                    />
                )
            } else {
                return (
                    <div key={i} className="chart-loader">
                        <i className="fa fa-spinner fa-spin fa-3x fa-fw"></i>
                        <span className="sr-only">Loading...</span>
                    </div>
                )
            }
        })
    }

    renderComparisonChart(Chart) {
        const formattedData = this.props.selected_counters.map((scId, i) => ({
            'id': scId,
            'name': this.props.counters.filter(c => c.id === scId)[0].name,
            'data': this.props.data[scId] || []
        }))

        if (formattedData && formattedData.length) {
            return (
                <Chart
                    chart-type={this.state.chartType}
                    title="Сравнение"
                    data={formattedData}
                />
            )
        } else {
            return (
                <div className="chart-loader">
                    <i className="fa fa-spinner fa-spin fa-3x fa-fw"></i>
                    <span className="sr-only">Loading...</span>
                </div>
            )
        }
    }

    render() {
        const Chart = this.state.chartComponent

        return (
            <div>
                <div className="menu-customisation">
                    <div>
                        <p>Choose Components Type</p>
                        <button className="btn" onClick={() => this.switchComponentType('Highcharts')}>Highcharts</button>
                        <button className="btn" onClick={() => this.switchComponentType('Canvas')}>Canvas</button>
                        <button className="btn" onClick={() => this.switchComponentType('D3')}>D3</button>
                    </div>
                    <div>
                        <p>Choose Chart Type</p>
                        <button className="btn" onClick={() => this.switchChartType('line')}>Line</button>
                        <button className="btn" onClick={() => this.switchChartType('column')}>Column</button>
                    </div>
                    <div>
                        <p>Toggle compare mode</p>
                        <button className="btn" onClick={() => this.toggleCompareMode()}>Compare mode</button>
                    </div>
                </div>
                {
                    this.state.compareMode ? this.renderComparisonChart(Chart) : this.renderSeparateCharts(Chart)
                }
            </div>
        )
    }
}

const mapStateToProps = state => {
    return {
        data: state.data,
        selected_counters: state.selected_counters,
        counters: state.counters
    }
}

export default connect(
    mapStateToProps
)(SeparateChartsPage)