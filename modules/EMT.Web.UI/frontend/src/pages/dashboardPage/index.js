import { connect } from 'react-redux'
import DashboardPage from './dashboard.component'

const mapStateToProps = state => {
    return {
        data: state.data,
        selected_counters: state.selected_counters,
        counters: state.counters,
        dateFrom: state.dateFrom,
        dateTo: state.dateTo
    }
}

export default connect(
    mapStateToProps
)(DashboardPage)