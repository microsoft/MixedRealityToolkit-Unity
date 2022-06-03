// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.MixedReality.Toolkit.Accessibility
{
    /// <summary>
    /// Roles are used to provide assistive technologies with information
    /// regarding the purpose of the component.
    /// </summary>
    /// <remarks>
    /// The values defined here are a subset of the W3C's Core Accessibility Mappings v1.2
    /// document's role mapping table <see href="https://www.w3.org/TR/core-aam-1.2/#mapping_role_table"/>.
    /// The roles defined are based on documented mappings from ARIA to Microsoft UI Automation
    /// <see cref="https://docs.microsoft.com/windows/win32/winauto/uiauto-ariaspecification#w3c-aria-role-mapped-to-microsoft-active-accessibility-and-ui-automation"/>.
    /// </remarks>
    public enum ComponentRole
    {
        /// <summary>
        /// This component has no role.
        /// </summary>
        None = 0,

        #region ARIA Roles

        /// <summary>
        /// A type of live region with important, and usually time-sensitive, information.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#alert"/> for more information.
        /// </summary>
        Alert = 1,

        /// <summary>
        /// A type of dialog that contains an alert message, where initial focus goes to an element
        /// within the dialog.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#alertdialog"/> for more information.
        /// </summary>
        AlertDialog,

        /// <summary>
        /// A structure containing one or more focusable elements requiring user input, such as
        /// keyboard or gesture events.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#application"/> for more information.
        /// </summary>
        Application,

        /// <summary>
        /// A region that contains mostly site-oriented content, rather than page-specific content.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#banner"/> for more information.
        /// </summary>
        Banner,

        /// <summary>
        /// An input that allows for user-triggered actions when clicked or pressed.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#button"/> for more information.
        /// </summary>
        Button,

        /// <summary>
        /// A checkable input that has three possible values: true, false, or mixed.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#checkbox"/> for more information.
        /// </summary>
        CheckBox,

        /// <summary>
        /// A cell containing header information for a column.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#columnheader"/> for more information.
        /// </summary>
        ColumnHeader,

        /// <summary>
        /// A composite widget containing a single-line textbox and another element, such as a listbox or grid,
        /// that can dynamically pop up to help the user set the value of the textbox.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#combobox"/> for more information.
        /// </summary>
        ComboBox,

        /// <summary>
        /// A supporting section of the document, designed to be complementary to the main content at a similar
        /// level in the DOM hierarchy, but remains meaningful when separated from the main content.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#complementary"/> for more information.
        /// </summary>
        Complementary,

        /// <summary>
        /// A large perceivable region that contains information about the parent document.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#contentinfo"/> for more information.
        /// </summary>
        ContentInfo,

        /// <summary>
        /// A definition of a term or concept.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#definition"/> for more information.
        /// </summary>
        Definition,

        // /// <summary>
        // /// Please see <see cref=""/> for more information.
        // /// </summary>
        // // todo: find documentation (beyond the reference at https://docs.microsoft.com/windows/win32/winauto/uiauto-ariaspecification#w3c-aria-role-mapped-to-microsoft-active-accessibility-and-ui-automation)
        // Description, 

        /// <summary>
        /// A dialog is a descendant window of the primary window of a web application.
        /// </summary>
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#dialog"/> for more information.
        Dialog,

        /// <summary>
        /// A list of references to members of a group, such as a static table of contents.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#directory"/> for more information.
        /// </summary>
        Directory,

        /// <summary>
        /// An element containing content that assistive technology users may want to browse in a
        /// reading mode.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#document"/> for more information.
        /// </summary>
        Document,

        /// <summary>
        /// A landmark region that contains a collection of items and objects that, as a whole,
        /// combine to create a form.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#form"/> for more information.
        /// </summary>
        Form,

        /// <summary>
        /// A composite widget containing a collection of one or more rows with one or more cells
        /// where some or all cells in the grid are focusable by using methods of two-dimensional
        /// navigation, such as directional arrow keys.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#grid"/> for more information.
        /// </summary>
        Grid,

        /// <summary>
        /// A cell in a grid or treegrid.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#gridcell"/> for more information.
        /// </summary>
        GridCell,

        /// <summary>
        /// A set of user interface objects which are not intended to be included in a page summary or
        /// table of contents by assistive technologies.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#group"/> for more information.
        /// </summary>
        Group,

        /// <summary>
        /// A heading for a section of the page.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#heading"/> for more information.
        /// </summary>
        Heading,

        /// <summary>
        /// A container for a collection of elements that form an image.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#image"/> for more information.
        /// </summary>
        Image,

        /// <summary>
        /// An interactive reference to an internal or external resource that, when activated, causes
        /// the user agent to navigate to that resource.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#link"/> for more information.
        /// </summary>
        Link,

        /// <summary>
        /// A section containing listitem elements.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#list"/> for more information.
        /// </summary>
        List,

        /// <summary>
        /// A widget that allows the user to select one or more items from a list of choices.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#listbox"/> for more information.
        /// </summary>
        ListBox,

        /// <summary>
        /// A single item in a list or directory.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#listitem"/> for more information.
        /// </summary>
        ListItem,

        /// <summary>
        /// A type of live region where new information is added in meaningful order and old
        /// information may disappear.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#log"/> for more information.
        /// </summary>
        Log,

        /// <summary>
        /// The main content of a document.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#main"/> for more information.
        /// </summary>
        Main,

        /// <summary>
        /// A type of live region where non-essential information changes frequently.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#marguee"/> for more information.
        /// </summary>
        Marquee,

        /// <summary>
        /// A type of widget that offers a list of choices to the user.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#menu"/> for more information.
        /// </summary>
        Menu,

        /// <summary>
        /// A presentation of menu that usually remains visible and is usually presented horizontally.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#menubar"/> for more information.
        /// </summary>
        MenuBar,

        /// <summary>
        /// An option in a set of choices contained by a menu or menubar.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#menuitem"/> for more information.
        /// </summary>
        MenuItem,

        /// <summary>
        /// A menuitem with a checkable state whose possible values are true, false, or mixed.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#menuitemcheckbox"/> for more information.
        /// </summary>
        MenuItemCheckBox,

        /// <summary>
        /// A checkable menuitem in a set of elements with the same role, only one of which can be
        /// checked at a time.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#menuitemradio"/> for more information.
        /// </summary>
        MenuItemRadio,

        /// <summary>
        /// A collection of navigational elements (usually links) for navigating the document or
        /// related documents.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#navigation"/> for more information.
        /// </summary>
        Navigation,

        /// <summary>
        /// A section whose content is parenthetic or ancillary to the main content of the resource.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#note"/> for more information.
        /// </summary>
        Note,

        /// <summary>
        /// A selectable item in a select list.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#option"/> for more information.
        /// </summary>
        Option,

        /// <summary>
        /// An element that displays the progress status for tasks that take a long time.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#progressbar"/> for more information.
        /// </summary>
        ProgressBar,

        /// <summary>
        /// A checkable input in a group of elements with the same role, only one of which can be checked
        /// at a time.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#radio"/> for more information.
        /// </summary>
        Radio,

        /// <summary>
        /// A group of radio buttons.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#radiogroup"/> for more information.
        /// </summary>
        RadioGroup,

        /// <summary>
        /// A perceivable section containing content that is relevant to a specific, author-specified
        /// purpose and sufficiently important that users will likely want to be able to navigate to the
        /// section easily and to have it listed in a summary of the page.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#region"/> for more information.
        /// </summary>
        Region,

        /// <summary>
        /// A row of cells in a tabular container.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#row"/> for more information.
        /// </summary>
        Row,

        /// <summary>
        /// A cell containing header information for a row in a grid.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#rowheader"/> for more information.
        /// </summary>
        RowHeader,

        /// <summary>
        /// A graphical object that controls the scrolling of content within a viewing area, regardless
        /// of whether the content is fully displayed within the viewing area.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#scrollbar"/> for more information.
        /// </summary>
        ScrollBar,

        /// <summary>
        /// A landmark region that contains a collection of items and objects that, as a whole, combine
        /// to create a search facility.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#search"/> for more information.
        /// </summary>
        Search,

        /// <summary>
        /// A renderable structural containment unit in a document or application.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#section"/> for more information.
        /// </summary>
        Section,

        /// <summary>
        /// A divider that separates and distinguishes sections of content or groups of menuitems.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#separator"/> for more information.
        /// </summary>
        Separator,

        /// <summary>
        /// A user input where the user selects a value from within a given range.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#slider"/> for more information.
        /// </summary>
        Slider,

        /// <summary>
        /// A form of range that expects the user to select from among discrete choices.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#spinbutton"/> for more information.
        /// </summary>
        SpinButton,

        /// <summary>
        /// A type of live region whose content is advisory information for the user but is not important
        /// enough to justify an alert, often but not necessarily presented as a status bar.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#status"/> for more information.
        /// </summary>
        Status,

        /// <summary>
        /// A grouping label providing a mechanism for selecting the tab content that is to be rendered
        /// to the user.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#tab"/> for more information.
        /// </summary>
        Tab,

        /// <summary>
        /// A list of tab elements, which are references to tabpanel elements.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#tablist"/> for more information.
        /// </summary>
        TabList,

        /// <summary>
        /// A container for the resources associated with a tab, where each tab is contained in a tablist.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#tabpanel"/> for more information.
        /// </summary>
        TabPanel,

        /// <summary>
        /// A type of input that allows free-form text as its value.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#textbox"/> for more information.
        /// </summary>
        TextBox,

        /// <summary>
        /// A type of live region containing a numerical counter which indicates an amount of elapsed
        /// time from a start point, or the time remaining until an end point.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#timer"/> for more information.
        /// </summary>
        Timer,

        /// <summary>
        /// A collection of commonly used function buttons or controls represented in compact visual form.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#toolbar"/> for more information.
        /// </summary>
        ToolBar,

        /// <summary>
        /// A contextual popup that displays a description for an element.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#tooltip"/> for more information.
        /// </summary>
        ToolTip,

        /// <summary>
        /// A type of list that may contain sub-level nested groups that can be collapsed and expanded.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#tree"/> for more information.
        /// </summary>
        Tree,

        /// <summary>
        /// A grid whose rows can be expanded and collapsed in the same manner as for a tree.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#treegrid"/> for more information.
        /// </summary>
        TreeGrid,

        /// <summary>
        /// An option item of a tree. This is an element within a tree that may be expanded or collapsed
        /// if it contains a sub-level group of tree item elements.
        /// Please see <see cref="https://www.w3.org/TR/wai-aria-1.1/#treeitem"/> for more information.
        /// </summary>
        TreeItem,

        #endregion ARIA Roles

        #region MRTK Custom Roles

        // To be defined

        #endregion MRTK Custom Roles
    }
}
