import React, {Component} from 'react';
import Heading from 'arui-feather/heading/';
import {connect} from "react-redux";
import {fetchResults} from "../../actions/results";
import Plate from "arui-feather/plate/";
import Paragraph from "arui-feather/paragraph/";
import styled from "@emotion/styled";
import Notification from "../misc/notification";

const Container = styled.div`
    margin-top: 24px;
    display: grid;
`;

const renderMedal = i => {
    if (i === 0) return <img src={"img/gold.svg"} alt="gold badge" className="badge"/>;
    else if (i === 1) return <img src={"img/silver.svg"} alt="silver badge" className="badge"/>;
    else if (i === 2) return <img src={"img/bronze.svg"} alt="bronze badge" className="badge"/>
};

const declOfNum = (number, titles) => {
    const cases = [2, 0, 1, 1, 1, 2];
    return titles[(number % 100 > 4 && number % 100 < 20) ? 2 : cases[(number % 10 < 5) ? number % 10 : 5]];
};

const secToMinHour = sec => {
    const h = sec / 3600 ^ 0;
    const m = (sec - h * 3600) / 60 ^ 0;
    const s = sec - h * 3600 - m * 60;

    let str = "";
    if (h > 0) str = `${h} ч. `;
    else str = "";
    if (m > 0) str = str + `${m} мин. `;

    str = str + `${s} сек. `;
    return str;
};

const renderNullResult = () =>
    (
        <div style={{"marginBottom": "10px"}}>
            <Plate hasCloser={false}>
                <Heading size='m'>
                    Пока нет игроков
                </Heading>
                <Paragraph>
                    Игроки появятся здесь позже
                </Paragraph>
            </Plate>
        </div>
    );

const ParagraphOutsider = styled(Paragraph)`
    margin: 0 0 0 !important;
`;

const renderResult = ({name, points, seconds, phone}, i) =>
    (
        <div style={{"marginBottom": "10px"}} key={i}>
            {i < 3 ? <Plate hasCloser={false}>
                    {renderMedal(i)}
                    <Heading size='m'>
                        {i + 1} место
                    </Heading>
                    <Heading size='s'>
                        {name} {phone}
                    </Heading>
                    <Paragraph>
                        Игрок заработал {points} {declOfNum(points, ["балл", "балла", "баллов"])} за {secToMinHour(seconds)}
                    </Paragraph>
                </Plate> :
                <Plate hasCloser={false}>
                    <ParagraphOutsider>
                        {i + 1} место
                        - {name} {phone} - {points} {declOfNum(points, ["балл", "балла", "баллов"])} за {secToMinHour(seconds)}
                    </ParagraphOutsider>
                </Plate>
            }
        </div>
    );

const renderResults = results => {
    if (results) {
        if (results.length === 0) {
            return renderNullResult();
        } else {
            return results.map((r, i) => renderResult(r, i));
        }
    } else {
        return <div/>;
    }
};

class ResultsContainer extends Component {
    intervalID = null;

    componentDidMount() {
        const {fetchResults, isActive} = this.props;

        if (isActive && isActive("/results")) {
            fetchResults();
        }

        this.intervalID = setInterval(() => {
            if (isActive && isActive("/results")) {
                fetchResults();
            }
        }, 5000);
    }

    componentWillUnmount() {
        clearInterval(this.intervalID);
    }

    render() {
        return (
            <Container>
                <Notification
                    title="Ошибка загрузки"
                    message="Что то произошло не так! Ошибка загрузки"
                    error={this.props.error}>
                </Notification>
                {renderResults(this.props.results)}
            </Container>
        );
    }
}

const mapStateToProps = state => ({
    results: state.results.data,
    error: state.results.error
});

const mapDispatchToProps = dispatch => ({
    fetchResults: () => dispatch(fetchResults())
});

export default connect(mapStateToProps, mapDispatchToProps)(ResultsContainer);