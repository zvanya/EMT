import { connect } from 'react-redux'

import ChartLinear from '../../components/chartHighchartLinear/'

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


class CompareChartsPage extends React.Component {
    constructor(props) {
        super(props)
    }

    render() {
        const formattedData = this.props.selected_counters.map((scId, i) => ({
            'id': scId,
            'name': this.props.counters.filter(c => c.id === scId)[0].name,
            'data': this.props.data[scId] || []
        }))

        if (formattedData && formattedData[0] && formattedData[0].data.length) {
            return (
                <ChartLinear
                    title="Compare"
                    data={formattedData}
                />
            )
        } else {
            return <span>Please select at least one counter</span>
        }
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
)(CompareChartsPage)