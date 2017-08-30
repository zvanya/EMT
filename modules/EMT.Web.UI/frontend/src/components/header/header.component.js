import './header.scss'
import DatePicker from 'react-datepicker'
import moment from 'moment'
import 'react-datepicker/dist/react-datepicker.css'

const Header = ({
    dateFrom,
    dateTo,
    handleDateFromChange,
    handleDateToChange
}) => (
    <header className="header-main">
        <span className="date-block">
            <span className="date-label">From:</span>
            <DatePicker
                selected={moment(dateFrom)}
                onChange={handleDateFromChange}
            />
        </span>
        <span className="date-block">
            <span className="date-label">To:</span>
            <DatePicker
                selected={moment(dateTo)}
                onChange={handleDateToChange}
            />
        </span>
    </header>
)

export default Header
